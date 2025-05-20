using BLL.API;
using DAL.Models;
using DAL.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IServiceProviderManagement _serviceProviderManagementDal;
        private readonly IAppointmentManagement _appointmentManagement;
        private readonly IAppointmentsSlotManagement _appointmentsSlotManagementDal;
        public AppointmentService(IAppointmentManagement appointmentManagement, IAppointmentsSlotManagement appointmentsSlotManagementDal, IServiceProviderManagement serviceProviderManagementDal)
        {
            _serviceProviderManagementDal = serviceProviderManagementDal;
            _appointmentManagement = appointmentManagement;
            _appointmentsSlotManagementDal = appointmentsSlotManagementDal;
        }
        public async Task<List<Appointment>> GetAppointmentsByProviderNameAsync(string doctorName, string clinicName = null)
        {
            if (string.IsNullOrEmpty(doctorName))
            {
                throw new ArgumentException("Provider name cannot be null or empty.");
            }

            var serviceProvider = await _serviceProviderManagementDal.GetIdProvideByName(doctorName);

            var appointments = await _appointmentsSlotManagementDal.GetAppointmentsByDoctorAndClinicAsync(serviceProvider, clinicName);

      
            if (appointments == null || appointments.Count == 0)
            {
                throw new InvalidOperationException("No appointments found for this service provider.");
            }

       
            return appointments;
        }
    }

        public async Task<List<Appointment>> GetAppointmentsByDoctorFilteredAsync(string doctorName, string clinicName = null)
        {
          
            if (string.IsNullOrWhiteSpace(doctorName))
                throw new ArgumentException("Doctor name is required.");

         
            var allAppointments = await _appointmentsSlotManagementDal.GetAppointmentsByDoctorAndClinicAsync(doctorName, null);

            if (allAppointments == null || allAppointments.Count == 0)
                throw new InvalidOperationException("No appointments found.");
            if (!string.IsNullOrWhiteSpace(clinicName))
            {
                allAppointments = allAppointments
                    .Where(a => a.ClinicName.Equals(clinicName, StringComparison.OrdinalIgnoreCase)) // השוואה ללא תלות באותיות קטנות/גדולות
                    .ToList();
            }

            // מחזירים את התורים (אחרי סינון אם היה)
            return allAppointments;
        }
        // ביטול תור לפי מזהה
        public async Task CancelAppointmentAsync(int appointmentId)
        {
            await _appointmentManagement.DeleteAppointment(appointmentId);
        }


        // שליפת כל התורים של משתמש לפי שם או מזהה משתמש
        public async Task<List<Appointment>> GetAppointmentsByUserAsync(string patientName)
        {
            return await _appointmentManagement.GetAppointmentsByPatientIdAsync(patientName);
        }


        //קביעת תור
        public async Task<bool> TryBookAppointmentAsync(Appointment appointment)
        {
            return await _appointmentManagement.TryAddAppointmentSafe(appointment);
        }


    }



}
