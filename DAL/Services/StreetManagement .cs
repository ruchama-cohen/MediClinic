using DAL.API;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Services
{
    public class StreetManagement : IStreetManagement
    {
        private readonly DB_Manager _context;

        public StreetManagement(DB_Manager context)
        {
            _context = context;
        }

        public async Task<int> AddStreet(Street street)
        {
            try
            {
                var existingStreet = await GetStreetByName(street.Name, street.CityId);
                if (existingStreet != null)
                {
                    return existingStreet.StreetId;
                }

                await _context.Streets.AddAsync(street);
                await _context.SaveChangesAsync();
                return street.StreetId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding street: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteStreet(int id)
        {
            try
            {
                var street = await _context.Streets.FindAsync(id);
                if (street == null)
                    return false;
                var hasAddresses = await _context.Addresses.AnyAsync(a => a.StreetId == id);
                if (hasAddresses)
                {
                    throw new InvalidOperationException("Cannot delete street with existing addresses");
                }

                _context.Streets.Remove(street);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting street: {ex.Message}");
                throw;
            }
        }

        public async Task<Street> GetOrAddStreetAsync(string streetName, int cityId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(streetName))
                    throw new ArgumentException("Street name cannot be empty");

                if (cityId <= 0)
                    throw new ArgumentException("Invalid city ID");

                var existingStreet = await GetStreetByName(streetName, cityId);
                if (existingStreet != null)
                    return existingStreet;

                var newStreet = new Street
                {
                    Name = streetName.Trim(),
                    CityId = cityId
                };

                await AddStreet(newStreet);
                return newStreet;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting or adding street: {ex.Message}");
                throw;
            }
        }

        public async Task<Street?> GetStreetById(int id)
        {
            try
            {
                return await _context.Streets
                    .Include(s => s.City)
                    .FirstOrDefaultAsync(s => s.StreetId == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting street by id: {ex.Message}");
                return null;
            }
        }

        public async Task<Street?> GetStreetByName(string name, int cityId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                    return null;

                return await _context.Streets
                    .Include(s => s.City)
                    .FirstOrDefaultAsync(s => s.Name.ToLower() == name.ToLower().Trim()
                                          && s.CityId == cityId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting street by name: {ex.Message}");
                return null;
            }
        }

        public async Task<List<Street>> GetStreetsByCity(int cityId)
        {
            try
            {
                return await _context.Streets
                    .Where(s => s.CityId == cityId)
                    .Include(s => s.City)
                    .OrderBy(s => s.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting streets by city: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> StreetExists(string name, int cityId)
        {
            try
            {
                return await _context.Streets
                    .AnyAsync(s => s.Name.ToLower() == name.ToLower() && s.CityId == cityId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking if street exists: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateStreet(Street street)
        {
            try
            {
                var existingStreet = await _context.Streets.FindAsync(street.StreetId);
                if (existingStreet == null)
                    return false;

                existingStreet.Name = street.Name;
                existingStreet.CityId = street.CityId;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating street: {ex.Message}");
                return false;
            }
        }
    }
}