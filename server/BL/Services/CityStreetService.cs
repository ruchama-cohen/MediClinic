using BLL.API;
using DAL.API;
using DAL.Models;

namespace BLL.Services
{
    public class CityStreetService : ICityStreetService
    {
        private readonly IAddressManagement _addressManagement;

        public CityStreetService(IAddressManagement addressManagement)
        {
            _addressManagement = addressManagement;
        }

        public async Task<List<City>> GetAllCitiesAsync()
        {
            // נשתמש ב-AddressManagement כדי לקבל רק ערים שיש להן כתובות
            // אם אין פונקציה כזו, נוסיף אותה
            return await _addressManagement.GetAllCitiesWithAddressesAsync();
        }

        public async Task<List<Street>> GetStreetsByCityIdAsync(int cityId)
        {
            if (cityId <= 0)
                throw new ArgumentException("City ID must be positive", nameof(cityId));

            return await _addressManagement.GetStreetsByCityIdAsync(cityId);
        }
    }
}