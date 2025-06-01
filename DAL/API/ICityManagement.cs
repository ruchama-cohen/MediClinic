using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;

namespace DAL.API
{
    public interface ICityManagement
    {
        Task<int> AddCity(City city);
        Task<City?> GetCityByName(string name);
        Task<City?> GetCityById(int id);
        Task<List<City>> GetAllCities();
        Task<bool> UpdateCity(City city);
        Task<bool> DeleteCity(int id);
        Task<City> GetOrAddCityAsync(string cityName);
    }
}
