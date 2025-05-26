using BLL.API;
using DAL.API;
using DAL.Models;

namespace BLL.Services
{
    public class AppointmentService : IAppointmentService,
    {
        private readonly IServiceProviderManagement _serviceProviderManagementDal;
        private readonly IAppointmentManagement _appointmentManagement;
        private readonly IAppointmentsSlotManagement _appointmentsSlotManagementDal;
        private readonly IPatientsManagement _patientsManagementDal;

        public AppointmentService(IAppointmentManagement appointmentManagement, IAppointmentsSlotManagement appointmentsSlotManagementDal, IServiceProviderManagement serviceProviderManagementDal, IPatientsManagement patientsManagementDal)
        {
            _serviceProviderManagementDal = serviceProviderManagementDal;
            _appointmentManagement = appointmentManagement;
            _appointmentsSlotManagementDal = appointmentsSlotManagementDal;
            _patientsManagementDal = patientsManagementDal;
        }

        //כאן עשיתי חיפוש רופא רק לפי שם
        public async Task<List<AppointmentsSlot>> GetAppointmentsByProviderNameAsync(string doctorName)
        {
            if (string.IsNullOrEmpty(doctorName))
            {
                throw new ArgumentException("Provider name cannot be null or empty.");
            }
            var providerId = await _serviceProviderManagementDal.GetProviderIDByName(doctorName);


            var appointments = await _appointmentsSlotManagementDal.GetAppointmentsSlotsByServiceProviderID(providerId);


            if (appointments == null || appointments.Count == 0)
            {
                throw new InvalidOperationException("No appointments found for this service provider.");
            }


            return appointments;
        }


        //חיפוש תור לפי שם רופא והעיר שלו
        public async Task<List<AppointmentsSlot>> GetAvailableSlotsByProviderAndCityAsync(string doctorName, string cityName)
        {

            if (string.IsNullOrEmpty(doctorName))
            {
                throw new ArgumentException("Provider name cannot be null or empty.");
            }
            var providerId = await _serviceProviderManagementDal.GetProviderIDByName(doctorName);
            var allSlots = await _appointmentsSlotManagementDal.GetAppointmentSlotByCityAndServiceName(providerId, cityName);
            if (allSlots == null || allSlots.Count == 0)
            {
                throw new InvalidOperationException("No appointments found for this service provider.");
            }

            return allSlots;
        }


        //חיפוש תור לפי מקצוע
        public async Task<List<AppointmentsSlot>> GetAvailableSlotsByServiceAsync(int serviceId)
        {
            var allSlots = await _appointmentsSlotManagementDal.GetAppointmentSlotByServiceType(serviceId);

            if (allSlots == null || allSlots.Count == 0)
            {
                throw new InvalidOperationException("No appointments found for this service provider.");
            }

            return allSlots;
        }



        // ביטול תור לפי מזהה
        public async Task CancelAppointmentAsync(int appointmentId)
        {
            await _appointmentManagement.DeleteAppointment(appointmentId);
        }


        // שליפת כל התורים של משתמש לפי שם או מזהה משתמש
        public async Task<List<Appointment>> GetAppointmentsByUserAsync(string patientName)
        {
            var patientKay = await _patientsManagementDal.GetPatientIDByName(patientName);

            return await _appointmentManagement.GetAppointmentsByPatientIdAsync(patientKay);
        }

        //קביעת תור לא בטוחה שהיא עובדת

        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public async Task<bool> BookAppointmentAsync(int slotId, Appointment appointment)
        {
            await _semaphore.WaitAsync();
            try
            {
                var slot = await _appointmentsSlotManagementDal.GetSlotByIdAsync(slotId);
                if (slot == null || slot.IsBooked)
                    return false;

                slot.IsBooked = true;
                appointment.SlotId = slotId;

                await _appointmentManagement.AddAppointment(appointment);

                return true;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        //
        public async Task<bool> GenerateSlotsForProviderAsync(int providerKey, DateOnly startDate, DateOnly endDate)
        {
            // 1. קבלת הספק עם שעות העבודה שלו
            var provider = await _serviceProviderManagementDal.GetProviderWithWorkHoursAsync(providerKey);
            if (provider == null || !provider.IsActive)
                return false;

            // 2. לולאה על כל יום בטווח התאריכים
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                // המרת יום בשבוע (1-7) לפי הצורך
                var weekday = (int)date.DayOfWeek;
                if (weekday == 0) weekday = 7;  // ב-C# יום ראשון הוא 0, כאן מניחים 7

                // 3. קבלת שעות עבודה ביום זה
                var workHours = provider.WorkHours
                    .Where(wh => wh.Weekday == weekday)
                    .ToList();

                // 4. עבור כל חלון זמן של עבודה - יצירת סלוטים
                foreach (var workHour in workHours)
                {
                    var currentTime = workHour.StartTime;
                    var meetingDuration = provider.MeetingTime ?? 30; // משך פגישה בדקות, ברירת מחדל 30

                    while (currentTime.AddMinutes(meetingDuration) <= workHour.EndTime)
                    {
                        // 5. בדיקת אם כבר קיים סלוט כזה
                        bool exists = await _appointmentsSlotManagementDal.AnySlotExistsAsync(providerKey, date, currentTime, provider.BranchId);
                        if (!exists)
                        {
                            // 6. יצירת סלוט חדש
                            var newSlot = new AppointmentsSlot
                            {
                                ProviderKey = providerKey,
                                SlotDate = date,
                                SlotStart = currentTime,
                                SlotEnd = currentTime.AddMinutes(meetingDuration),
                                BranchId = provider.BranchId,
                                IsBooked = false
                            };

                            await _appointmentsSlotManagementDal.AddAppointmentSlot(newSlot);
                        }

                        currentTime = currentTime.AddMinutes(meetingDuration);
                    }
                }
            }

            // 7. שמירת השינויים בבסיס הנתונים
            //לבדוק מה לעשות כאילו איפה  הכי נכון לשמור את הנתונים
            //  await _repository.SaveChangesAsync();

            return true;
        }





    }



}
