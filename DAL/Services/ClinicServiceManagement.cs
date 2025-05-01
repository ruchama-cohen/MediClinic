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

        public ClinicServiceManagement()
        {
            _context = new DB_Manager();
        }
        public async Task AddClinicService(ClinicService clinicService)
        {
            await _context.ClinicServices.AddAsync(clinicService);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteClinicService(string serviceName)
        {
            var clinicService = await _context.ClinicServices
                .FirstOrDefaultAsync(s => s.ServiceName == serviceName);

            if (clinicService != null)
            {
                _context.ClinicServices.Remove(clinicService);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
       

       
    }
}
