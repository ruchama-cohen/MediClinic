using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.API
{
    public interface IAppointmentService
    {
        Task<List<AppointmentsSlot>> GetAppointmentsByProviderNameAsync(string doctorName);
        Task<List<AppointmentsSlot>> GetAvailableSlotsByProviderAndCityAsync(string doctorName, string cityName);
        Task<List<AppointmentsSlot>> GetAvailableSlotsByServiceAsync(int serviceId);
        Task CancelAppointmentAsync(int appointmentId);
        Task<List<Appointment>> GetAppointmentsByUserAsync(string patientName);
        Task<bool> BookAppointmentAsync(int slotId, string patientId);
        Task<bool> GenerateSlotsForProviderAsync(int providerKey, DateOnly startDate, DateOnly endDate);
        Task<List<Appointment>> GetAppointmentsByPatientIdAsync(int patientKey);

    }
}
