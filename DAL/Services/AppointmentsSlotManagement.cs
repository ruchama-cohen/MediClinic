using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        }


        public async Task<bool> DeleteAppointmentSlot(int appointmentId)
        {
            var appointmentSlot = await _context.AppointmentsSlots.FindAsync(appointmentId);
            if (appointmentSlot == null)
            {
                return false;
            }

            _context.AppointmentsSlots.Remove(appointmentSlot);
            await _context.SaveChangesAsync();
            return true; // Slot successfully deleted
        }





        public async Task<List<AppointmentsSlot>?> GetAppointmentSlotByCityAndServiceName(int serviceId, string cityName)
        {
            return await _context.AppointmentsSlots
                .Where(a => a.Provider != null &&
                            a.Provider.ServiceId == serviceId &&
                            a.Branch != null &&
                            a.Branch.Address != null &&
                            a.Branch.Address.City == cityName)
                .ToListAsync();
        }




        public async Task<List<AppointmentsSlot>?> GetAppointmentSlotByServiceName(int serviceType)
        {
            return await _context.AppointmentsSlots
                .Where(a => a.Provider != null && a.Provider.ServiceId == serviceType)
                .ToListAsync();
        }
        public async Task<List<AppointmentsSlot>?> GetAppointmentSlotByServiceProviderIDAndCity(int serviceProviderID, string cityName)
        {
            return await _context.AppointmentsSlots
                .Where(a => a.Provider != null &&
                            a.Provider.ProviderId == serviceProviderID &&
                            a.Branch != null &&
                            a.Branch.Address != null &&
                            a.Branch.Address.City == cityName)
                .ToListAsync();
        }

        public async Task<List<AppointmentsSlot>?> GetAppointmentsSlotsByServiceProviderID(int serviceProviderID)
        {
            return await _context.AppointmentsSlots.Where(a => a.Provider != null && a.Provider.ProviderId == serviceProviderID).ToListAsync();
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


        public async Task<bool> UpdateQueueAvailability(int appointmentId)
        {
            var appointmentsSlot = await _context.AppointmentsSlots.FindAsync(appointmentId);
            if (appointmentsSlot == null) return false;
            _context.Entry(appointmentsSlot).CurrentValues.SetValues(updatedAppointmentsSlot);
            await _context.SaveChangesAsync();
            return true;

        }

    }
}
