using DAL.API;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Services
{
    public class AppointmentsSlotManagement : IAppointmentsSlotManagement
    {
        private readonly DB_Manager _context;

        public AppointmentsSlotManagement(DB_Manager context)
        {
            _context = context;
        }

        public async Task AddAppointmentSlot(AppointmentsSlot appointmentSlot)
        {
            _context.AppointmentsSlots.Add(appointmentSlot);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAppointmentSlot(int appointmentId)
        {
            var appointmentSlot = await _context.AppointmentsSlots.FindAsync(appointmentId);
            if (appointmentSlot == null)
                return false;

            _context.AppointmentsSlots.Remove(appointmentSlot);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<AppointmentsSlot>?> GetAppointmentSlotByCityAndServiceName(int serviceId, string cityName)
        {
            return await _context.AppointmentsSlots
                .Where(a =>
                    a.ProviderKeyNavigation != null &&
                    a.ProviderKeyNavigation.ServiceId == serviceId &&
                    a.Branch != null &&
                    a.Branch.Address != null &&
                    a.Branch.Address.City != null &&
                    a.Branch.Address.City.Name == cityName)
                .ToListAsync();
        }

        public async Task<List<AppointmentsSlot>?> GetAppointmentSlotByServiceType(int serviceType)
        {
            return await _context.AppointmentsSlots
                .Where(a => a.ProviderKeyNavigation != null && a.ProviderKeyNavigation.ServiceId == serviceType)
                .ToListAsync();
        }

        public async Task<List<AppointmentsSlot>?> GetAppointmentSlotByServiceProviderIDAndCityID(int serviceProviderKey, int cityID)
        {
            return await _context.AppointmentsSlots
                .Where(a => a.ProviderKeyNavigation != null &&
                            a.ProviderKeyNavigation.ProviderKey == serviceProviderKey &&
                            a.Branch != null &&
                            a.Branch.Address != null &&
                            a.Branch.Address.City.CityId == cityID)
                .ToListAsync();
        }

        public async Task<List<AppointmentsSlot>> GetAppointmentsByServiceProviderIDAndBranchID(int serviceProviderKey, int branchID)
        {
            return await _context.AppointmentsSlots
            .Where(a => a.ProviderKeyNavigation != null &&
                            a.ProviderKeyNavigation.ProviderKey == serviceProviderKey &&
                            a.Branch != null &&
                             a.Branch.BranchId == branchID)
                .ToListAsync();
        }

        public async Task<List<AppointmentsSlot>?> GetAppointmentsSlotsByServiceProviderID(int serviceProviderKey)
        {
            return await _context.AppointmentsSlots
                .Where(a => a.ProviderKeyNavigation != null && a.ProviderKeyNavigation.ProviderKey == serviceProviderKey)
                .ToListAsync();
        }

        public async Task<bool> UpdateAppointmentSlotDetails(AppointmentsSlot updatedAppointmentsSlot)
        {
            var appointmentsSlot = await _context.AppointmentsSlots.FindAsync(updatedAppointmentsSlot.BranchId);
            if (appointmentsSlot == null)
                return false;

            _context.Entry(appointmentsSlot).CurrentValues.SetValues(updatedAppointmentsSlot);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateQueueAvailability(int appointmentId, bool isBooked)
        {
            var appointmentsSlot = await _context.AppointmentsSlots.FindAsync(appointmentId);
            if (appointmentsSlot == null)
                return false;

            appointmentsSlot.IsBooked = isBooked;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<AppointmentsSlot?> GetSlotByIdAsync(int slotId)
        {
            return await _context.AppointmentsSlots
                .Include(a => a.ProviderKeyNavigation)
                .FirstOrDefaultAsync(a => a.SlotId == slotId);
        }

        public Task<bool> AnySlotExistsAsync(int providerKey, DateOnly date, TimeOnly startTime, int branchId)
        {
            return _context.AppointmentsSlots
                .AnyAsync(a => a.ProviderKeyNavigation != null &&
                               a.ProviderKeyNavigation.ProviderKey == providerKey &&
                               a.SlotDate == date &&
                               a.SlotStart == startTime &&
                               a.BranchId == branchId);
        }
    }
}