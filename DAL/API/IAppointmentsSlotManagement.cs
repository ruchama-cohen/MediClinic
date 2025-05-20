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
        Task<List<AppointmentsSlot>?> GetAppointmentSlotByCityAndServiceName(int serviceName, string cityName);
        Task<List<AppointmentsSlot>?> GetAppointmentsSlotsByServiceProviderID(int serviceProviderID);
        Task<List<AppointmentsSlot>?> GetAppointmentSlotByServiceName(int serviceName);
        Task<List<AppointmentsSlot>?> GetAppointmentSlotByServiceProviderIDAndCity(int serviceProviderID, string cityName);
        Task<bool> UpdateQueueAvailability(int appointmentId);

    }
}
