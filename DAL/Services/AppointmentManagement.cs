using DAL.API;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Services
{
    public class AppointmentManagement : IAppointmentManagement
    {
        private readonly DB_Manager _context;

        public AppointmentManagement(DB_Manager context)
        {
            _context = context;
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

            if (appointment == null)
                return false;

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Appointment>> GetAppointmentsByPatientIdAsync(int patientKey)
        {
            return await _context.Appointments
                .Where(a => a.PatientKeyNavigation.PatientKey == patientKey)
                .Include(a => a.Slot)
                    .ThenInclude(s => s.Branch)
                .Include(a => a.Slot)
                    .ThenInclude(s => s.ProviderKeyNavigation)
                .ToListAsync();
        }

        public async Task<Appointment?> GetAppointmentByIdAsync(int appointmentId)
        {
            return await _context.Appointments
                .Include(a => a.Slot)
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);
        }
    }
}