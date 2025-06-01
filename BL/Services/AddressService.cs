using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.API;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Services
{
    public class AddressManagement : IAddressManagement
    {
        private readonly DB_Manager _context;

        public AddressManagement(DB_Manager context)
        {
            _context = context;
        }

        public async Task<int> AddAddress(Address address)
        {
            try
            {
                await _context.Addresses.AddAsync(address);
                await _context.SaveChangesAsync();
                return address.AddressId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding address: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteAddress(int id)
        {
            try
            {
                var address = await _context.Addresses.FindAsync(id);
                if (address != null)
                {
                    _context.Addresses.Remove(address);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting address: {ex.Message}");
                return false;
            }
        }

        public async Task<string?> GetCityNameById(int id)
        {
            try
            {
                return await _context.Addresses
                    .Where(x => x.AddressId == id)
                    .Select(x => x.City.Name)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting city name: {ex.Message}");
                return null;
            }
        }

        public async Task<Address?> GetAddressById(int id)
        {
            try
            {
                return await _context.Addresses
                    .Include(a => a.City)
                    .Include(a => a.Street)
                    .FirstOrDefaultAsync(x => x.AddressId == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting address by id: {ex.Message}");
                return null;
            }
        }

        // Methods חדשים לטיפול בכתובות
        public async Task<City> GetOrCreateCityAsync(string cityName)
        {
            try
            {
                Console.WriteLine($"Getting or creating city: {cityName}");

                // חיפוש עיר קיימת
                var existingCity = await _context.Cities
                    .FirstOrDefaultAsync(c => c.Name.ToLower() == cityName.ToLower().Trim());

                if (existingCity != null)
                {
                    Console.WriteLine($"Found existing city: {existingCity.Name} (ID: {existingCity.CityId})");
                    return existingCity;
                }

                // יצירת עיר חדשה
                var newCity = new City { Name = cityName.Trim() };
                await _context.Cities.AddAsync(newCity);
                await _context.SaveChangesAsync();

                Console.WriteLine($"Created new city: {newCity.Name} (ID: {newCity.CityId})");
                return newCity;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting or creating city: {ex.Message}");
                throw;
            }
        }

        public async Task<Street> GetOrCreateStreetAsync(string streetName, int cityId)
        {
            try
            {
                Console.WriteLine($"Getting or creating street: {streetName} in city ID: {cityId}");

                // חיפוש רחוב קיים
                var existingStreet = await _context.Streets
                    .FirstOrDefaultAsync(s => s.Name.ToLower() == streetName.ToLower().Trim() && s.CityId == cityId);

                if (existingStreet != null)
                {
                    Console.WriteLine($"Found existing street: {existingStreet.Name} (ID: {existingStreet.StreetId})");
                    return existingStreet;
                }

                // יצירת רחוב חדש
                var newStreet = new Street
                {
                    Name = streetName.Trim(),
                    CityId = cityId
                };
                await _context.Streets.AddAsync(newStreet);
                await _context.SaveChangesAsync();

                Console.WriteLine($"Created new street: {newStreet.Name} (ID: {newStreet.StreetId})");
                return newStreet;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting or creating street: {ex.Message}");
                throw;
            }
        }

        public async Task<Address?> FindExistingAddressAsync(int cityId, int streetId, int houseNumber, string postalCode)
        {
            try
            {
                Console.WriteLine($"Looking for existing address: CityId={cityId}, StreetId={streetId}, House={houseNumber}, Postal={postalCode}");

                var address = await _context.Addresses
                    .Include(a => a.City)
                    .Include(a => a.Street)
                    .FirstOrDefaultAsync(a =>
                        a.CityId == cityId &&
                        a.StreetId == streetId &&
                        a.HouseNumber == houseNumber &&
                        a.PostalCode == postalCode.Trim());

                Console.WriteLine($"Existing address found: {address != null}");
                return address;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error finding existing address: {ex.Message}");
                return null;
            }
        }

        public async Task<int> CreateFullAddressAsync(string cityName, string streetName, int houseNumber, string postalCode)
        {
            try
            {
                Console.WriteLine($"=== CREATING FULL ADDRESS ===");
                Console.WriteLine($"City: '{cityName}', Street: '{streetName}', House: {houseNumber}, Postal: '{postalCode}'");

                // שלב 1: יצירה או קבלת עיר
                var city = await GetOrCreateCityAsync(cityName);

                // שלב 2: יצירה או קבלת רחוב
                var street = await GetOrCreateStreetAsync(streetName, city.CityId);

                // שלב 3: בדיקה אם כתובת זהה כבר קיימת
                var existingAddress = await FindExistingAddressAsync(city.CityId, street.StreetId, houseNumber, postalCode);
                if (existingAddress != null)
                {
                    Console.WriteLine($"Using existing address ID: {existingAddress.AddressId}");
                    return existingAddress.AddressId;
                }

                // שלב 4: יצירת כתובת חדשה
                var newAddress = new Address
                {
                    CityId = city.CityId,
                    StreetId = street.StreetId,
                    HouseNumber = houseNumber,
                    PostalCode = postalCode.Trim()
                };

                await _context.Addresses.AddAsync(newAddress);
                await _context.SaveChangesAsync();

                Console.WriteLine($"Created new address ID: {newAddress.AddressId}");
                return newAddress.AddressId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating full address: {ex.Message}");
                throw;
            }
        }
    }
}