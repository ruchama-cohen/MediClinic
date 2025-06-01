using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.API
{
    public interface IStreetManagement
    {
        Task<int> AddStreet(Street street);
        Task<Street?> GetStreetByName(string name, int cityId);
        Task<Street?> GetStreetById(int id);
        Task<List<Street>> GetStreetsByCity(int cityId);
        Task<bool> UpdateStreet(Street street);
        Task<bool> DeleteStreet(int id);
        Task<bool> StreetExists(string name, int cityId);
        Task<Street> GetOrAddStreetAsync(string streetName, int cityId);
    }
}