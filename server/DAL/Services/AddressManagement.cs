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
            await _context.Addresses.AddAsync(address);
            await _context.SaveChangesAsync();
            return address.AddressId;
        }

        public async Task<bool> DeleteAddress(int id)
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

        public async Task<string?> GetCityNameById(int id)
        {
            return await _context.Addresses
                .Where(x => x.AddressId == id)
                .Select(x => x.City.Name)
                .FirstOrDefaultAsync();
        }

        public async Task<Address?> GetAddressById(int id)
        {
            return await _context.Addresses
                .Include(a => a.City)
                .Include(a => a.Street)
                .FirstOrDefaultAsync(x => x.AddressId == id);
        }

        public async Task<City> GetOrCreateCityAsync(string cityName)
        {
            var existingCity = await _context.Cities
                .FirstOrDefaultAsync(c => c.Name.ToLower() == cityName.ToLower().Trim());

            if (existingCity != null)
                return existingCity;

            var newCity = new City { Name = cityName.Trim() };
            await _context.Cities.AddAsync(newCity);
            await _context.SaveChangesAsync();

            return newCity;
        }

        public async Task<Street> GetOrCreateStreetAsync(string streetName, int cityId)
        {
            var existingStreet = await _context.Streets
                .FirstOrDefaultAsync(s => s.Name.ToLower() == streetName.ToLower().Trim() && s.CityId == cityId);

            if (existingStreet != null)
                return existingStreet;

            var newStreet = new Street
            {
                Name = streetName.Trim(),
                CityId = cityId
            };
            await _context.Streets.AddAsync(newStreet);
            await _context.SaveChangesAsync();

            return newStreet;
        }

        public async Task<Address?> FindExistingAddressAsync(int cityId, int streetId, int houseNumber, string postalCode)
        {
            return await _context.Addresses
                .Include(a => a.City)
                .Include(a => a.Street)
                .FirstOrDefaultAsync(a =>
                    a.CityId == cityId &&
                    a.StreetId == streetId &&
                    a.HouseNumber == houseNumber &&
                    a.PostalCode.ToLower() == postalCode.Trim().ToLower());
        }

        public async Task<int> CreateFullAddressAsync(string cityName, string streetName, int houseNumber, string postalCode)
        {
            var city = await GetOrCreateCityAsync(cityName);
            var street = await GetOrCreateStreetAsync(streetName, city.CityId);
            var existingAddress = await FindExistingAddressAsync(city.CityId, street.StreetId, houseNumber, postalCode);

            if (existingAddress != null)
                return existingAddress.AddressId;

            var newAddress = new Address
            {
                CityId = city.CityId,
                StreetId = street.StreetId,
                HouseNumber = houseNumber,
                PostalCode = postalCode.Trim()
            };

            await _context.Addresses.AddAsync(newAddress);
            await _context.SaveChangesAsync();

            return newAddress.AddressId;
        }

        public async Task<List<City>> GetAllCitiesWithAddressesAsync()
        {
            return await _context.Cities
                .Where(c => c.Addresses.Any()) // רק ערים שיש להן כתובות
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<List<Street>> GetStreetsByCityIdAsync(int cityId)
        {
            return await _context.Streets
                .Where(s => s.CityId == cityId && s.Addresses.Any()) // רק רחובות שיש להם כתובות
                .OrderBy(s => s.Name)
                .ToListAsync();
        }
    }
}
