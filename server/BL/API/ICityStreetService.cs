using DAL.Models;

namespace BLL.API
{
    public interface ICityStreetService
    {
        Task<List<City>> GetAllCitiesAsync();
        Task<List<Street>> GetStreetsByCityIdAsync(int cityId);
    }
}