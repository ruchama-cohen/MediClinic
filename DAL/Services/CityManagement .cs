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
    public class CityManagement : ICityManagement
    {
        private readonly DB_Manager _context;

        public CityManagement(DB_Manager context)
        {
            _context = context;
        }

        public async Task<int> AddCity(City city)
        {
            try
            {
                var existingCity = await GetCityByName(city.Name);
                if (existingCity != null)
                {
                    return existingCity.CityId;
                }

                await _context.Cities.AddAsync(city);
                await _context.SaveChangesAsync();
                return city.CityId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding city: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> CityExists(string name)
        {
            try
            {
                return await _context.Cities
                    .AnyAsync(c => c.Name.ToLower() == name.ToLower());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking if city exists: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteCity(int id)
        {
            try
            {
                var city = await _context.Cities.FindAsync(id);
                if (city == null)
                    return false;
                var hasAddresses = await _context.Addresses.AnyAsync(a => a.CityId == id);
                if (hasAddresses)
                {
                    throw new InvalidOperationException("Cannot delete city with existing addresses");
                }

                _context.Cities.Remove(city);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting city: {ex.Message}");
                throw;
            }
        }

        public async Task<List<City>> GetAllCities()
        {
            try
            {
                return await _context.Cities
                    .OrderBy(c => c.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting all cities: {ex.Message}");
                throw;
            }
        }

        public async Task<City?> GetCityById(int id)
        {
            try
            {
                return await _context.Cities
                    .FirstOrDefaultAsync(c => c.CityId == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting city by id: {ex.Message}");
                return null;
            }
        }

        public async Task<City?> GetCityByName(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                    return null;

                return await _context.Cities
                    .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower().Trim());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting city by name: {ex.Message}");
                return null;
            }
        }

        public async Task<City> GetOrAddCityAsync(string cityName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cityName))
                    throw new ArgumentException("City name cannot be empty");

                var existingCity = await GetCityByName(cityName);
                if (existingCity != null)
                    return existingCity;

                var newCity = new City { Name = cityName.Trim() };
                await AddCity(newCity);
                return newCity;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting or adding city: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateCity(City city)
        {
            try
            {
                var existingCity = await _context.Cities.FindAsync(city.CityId);
                if (existingCity == null)
                    return false;

                existingCity.Name = city.Name;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating city: {ex.Message}");
                return false;
            }
        }
    }
}
