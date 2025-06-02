using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DAL.API;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Services
{
    public class ClinicServiceManagement : IClinicServiceManagement
    {
        private readonly DB_Manager _context;

        public ClinicServiceManagement(DB_Manager context)
        {
            _context = context;
        }

        public async Task<List<ClinicService>> GetAllServices()
        {
            try
            {
                return await _context.ClinicServices
                    .OrderBy(s => s.ServiceName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting all services: {ex.Message}");
                throw;
            }
        }


        public async Task<bool> ServiceExists(int serviceId)
        {
            try
            {
                return await _context.ClinicServices
                    .AnyAsync(s => s.ServiceId == serviceId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking if service exists: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateService(ClinicService service)
        {
            try
            {
                var existingService = await _context.ClinicServices.FindAsync(service.ServiceId);
                if (existingService == null)
                    return false;

                existingService.ServiceName = service.ServiceName.Trim();
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating service: {ex.Message}");
                return false;
            }
        }
    }
}