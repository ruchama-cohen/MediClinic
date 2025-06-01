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

        public async Task AddClinicService(ClinicService clinicService)
        {
            try
            {
                // בדיקה אם השירות כבר קיים
                var existingService = await GetServiceByName(clinicService.ServiceName);
                if (existingService != null)
                {
                    throw new InvalidOperationException($"Service '{clinicService.ServiceName}' already exists");
                }

                await _context.ClinicServices.AddAsync(clinicService);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding clinic service: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteClinicService(int serviceID)
        {
            try
            {
                var clinicService = await _context.ClinicServices
                    .FirstOrDefaultAsync(s => s.ServiceId == serviceID);

                if (clinicService != null)
                {
                    // בדיקה אם יש רופאים המשויכים לשירות זה
                    var hasProviders = await _context.ServiceProviders.AnyAsync(sp => sp.ServiceId == serviceID);
                    if (hasProviders)
                    {
                        throw new InvalidOperationException("Cannot delete service with existing providers");
                    }

                    _context.ClinicServices.Remove(clinicService);
                    await _context.SaveChangesAsync();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting clinic service: {ex.Message}");
                throw;
            }
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

        public async Task<ClinicService?> GetServiceById(int serviceId)
        {
            try
            {
                return await _context.ClinicServices
                    .FirstOrDefaultAsync(s => s.ServiceId == serviceId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting service by id: {ex.Message}");
                return null;
            }
        }

        public async Task<ClinicService?> GetServiceByName(string serviceName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(serviceName))
                    return null;

                return await _context.ClinicServices
                    .FirstOrDefaultAsync(s => s.ServiceName.ToLower() == serviceName.ToLower().Trim());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting service by name: {ex.Message}");
                return null;
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