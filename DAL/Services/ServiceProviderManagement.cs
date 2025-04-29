using DAL.API;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Services
{
    internal class ServiceProviderManagement : IServiceProviderManagement
    {
        private readonly DB_Manager _context;

        public ServiceProviderManagement()
        {
            _context = new DB_Manager();
        }
        public async Task AddServiceProvider(ServiceProvider serviceProvider)
        {
            await _context.ServiceProviders.AddAsync(serviceProvider);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteServiceProvider(int providerId)
        {
            var provider = await _context.ServiceProviders.FindAsync(providerId);
            if (provider != null)
            {
                _context.ServiceProviders.Remove(provider);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateServiceProviderDetails(ServiceProvider serviceProvider)
        {
            var serviceProviderN = await _context.ServiceProviders.FindAsync(serviceProvider.WorkHourId);

            if (serviceProviderN == null)
                throw new Exception("ServiceProvider not found");

            _context.Entry(serviceProviderN).CurrentValues.SetValues(serviceProvider);

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
