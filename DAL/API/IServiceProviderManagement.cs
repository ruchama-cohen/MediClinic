using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.API
{
    public interface IServiceProviderManagement
    {
        Task AddServiceProvider(ServiceProvider serviceProvider);
        Task<bool> UpdateServiceProviderDetails(ServiceProvider serviceProvider);
        Task<bool> DeleteServiceProvider(int providerId);
        Task<bool> UpdateServiceProvidersAvailability(int providerId);
        Task<List<ServiceProvider>> GetAllServiceProvidersByServiceId(int serviceId);
        Task<int> GetProviderKeyByName(string name);
        Task<ServiceProvider?> GetProviderWithWorkHoursAsync(int providerKey);
    }
}
