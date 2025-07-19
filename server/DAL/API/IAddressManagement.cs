using DAL.Models;

namespace DAL.API
{
    public interface IAddressManagement
    {
        Task<int> AddAddress(Address address);
        Task<Address?> GetAddressById(int id);
        Task<bool> DeleteAddress(int id);
        Task<string?> GetCityNameById(int id);
        Task<City> GetOrCreateCityAsync(string cityName);
        Task<Street> GetOrCreateStreetAsync(string streetName, int cityId);
        Task<Address?> FindExistingAddressAsync(int cityId, int streetId, int houseNumber, string postalCode);
        Task<int> CreateFullAddressAsync(string cityName, string streetName, int houseNumber, string postalCode);

        // הוספנו פונקציות חדשות
        Task<List<City>> GetAllCitiesWithAddressesAsync();
        Task<List<Street>> GetStreetsByCityIdAsync(int cityId);
    }
}