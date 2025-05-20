using DAL.API;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
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

        public AppointmentManagement(DB_Manager context)
        {
            _context = context;
        }

        public async Task<bool> TryAddAppointmentSafe(Appointment appointment)
        {
            try
            {
                await _context.Appointments.AddAsync(appointment);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                return false;
            }
        }

        public async Task AddAppointment(Appointment appointment)
        {
            await _context.Appointments.AddAsync(appointment);

            await _context.SaveChangesAsync();
        }


        public async Task<bool> DeleteAppointment(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Slot)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (appointment != null)
            {
                appointment.Slot.IsBooked = false;
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }


        public async Task<List<Appointment>> GetAppointmentsByPatientIdAsync(string patientId)
        {
            return await _context.Appointments
                .Where(a => a.PatientKeyNavigation.PatientId == patientId)
                .Include(a => a.Slot)
                    .ThenInclude(s => s.Branch)
                .Include(a => a.Slot)
                    .ThenInclude(s => s.ProviderKeyNavigation)
                .ToListAsync();
        }




    }
}
