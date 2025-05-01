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
    public class AppointmentManagement : IAppointmentManagement
    {
        private readonly DB_Manager _context;

        public AppointmentManagement()
        {
            _context = new DB_Manager();
        }
        public async Task AddAppointment(Appointment appointment)
        {
            await _context.Appointments.AddAsync(appointment);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
