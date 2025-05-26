using DAL.API;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ServiceProvider = DAL.Models.ServiceProvider;
namespace DAL.Services
{
    public class ServiceProviderManagement : IServiceProviderManagement
    {
        private readonly DB_Manager _context;

        public ServiceProviderManagement(DB_Manager context)
        {
            _context = context;
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
            // Find the existing service provider by its primary key (ProviderId)
            var serviceProviderN = await _context.ServiceProviders.FindAsync(serviceProvider.ProviderId);

            if (serviceProviderN == null)
                return false; // If not found, return false

            // Update the tracked entity with the new values
            _context.Entry(serviceProviderN).CurrentValues.SetValues(serviceProvider);

            // Save changes to the database
            await _context.SaveChangesAsync();
            return true; // Update successful
        }

        public async Task<bool> UpdateServiceProvidersAvailability(int providerKey)
        {
            var serviceProvider = await _context.ServiceProviders.FindAsync(providerKey);
            if (serviceProvider == null)
                return false;
            serviceProvider.IsActive = !serviceProvider.IsActive;
            return true;

        }
        public async Task<List<ServiceProvider>> GetAllServiceProvidersByServiceId(int serviceId)
        {
            var serviceProviders = await _context.ServiceProviders
                .Where(sp => sp.ServiceId == serviceId)
                .ToListAsync();

            return serviceProviders;
        }

        public async  Task<int> GetProviderKeyByName(string name)
        {
            var serviceProvider = await _context.ServiceProviders
                           .FirstOrDefaultAsync(sp => sp.Name == name);

            return serviceProvider.ProviderKey;
        }


        public async Task<ServiceProvider?> GetProviderWithWorkHoursAsync(int providerKey)
        {
            return await _context.ServiceProviders
                .Include(p => p.WorkHours)
                .FirstOrDefaultAsync(p => p.ProviderKey == providerKey);
        }

       
    }
}
