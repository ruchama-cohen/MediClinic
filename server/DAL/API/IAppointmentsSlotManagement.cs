using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;
namespace DAL.API
{
    public interface IAppointmentsSlotManagement
    {
        Task AddAppointmentSlot(AppointmentsSlot appointmentSlot);
        Task<bool> DeleteAppointmentSlot(int appointmentId);

        Task<bool> UpdateAppointmentSlotDetails(AppointmentsSlot appointmentSlot);
        Task<List<AppointmentsSlot>?> GetAppointmentSlotByCityAndServiceName(int serviceName, string cityID);
        Task<List<AppointmentsSlot>?> GetAppointmentsSlotsByServiceProviderID(int serviceProviderID);
        Task<List<AppointmentsSlot>?> GetAppointmentSlotByServiceType(int serviceName);
        Task<List<AppointmentsSlot>?> GetAppointmentSlotByServiceProviderIDAndCityID(int serviceProviderID, int cityID);
        Task<bool> UpdateQueueAvailability(int appointmentId,bool isBooked);
        Task<AppointmentsSlot?> GetSlotByIdAsync(int slotId);
        Task<bool> AnySlotExistsAsync(int providerKey, DateOnly date, TimeOnly startTime, int branchId);
    }
}
