using BLL.API;
using BLL.Exceptions;
using DAL.API;
using DAL.Models;
namespace BLL.Services
{
    public class AppointmentService : IAppointmentService
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
            if (string.IsNullOrWhiteSpace(doctorName))
            {
                throw new InvalidAppointmentDataException("DoctorName");
            }
            try
            {
                var providerId = await _serviceProviderManagementDal.GetProviderKeyByName(doctorName);
                if (providerId <= 0)
                {
                    throw new DoctorNotFoundException(doctorName);
                }
                var appointments = await _appointmentsSlotManagementDal.GetAppointmentsSlotsByServiceProviderID(providerId);

                if (appointments == null || appointments.Count == 0)
                {
                    throw new NoAvailableSlotsException($"for doctor {doctorName}");
                }
                var availableSlots = appointments
                    .Where(a => !a.IsBooked && a.SlotDate >= DateOnly.FromDateTime(DateTime.Now))
                    .ToList();
                if (!availableSlots.Any())
                {
                    throw new NoAvailableSlotsException($"for doctor {doctorName}");
                }
                return availableSlots;
            }
            catch (Exception ex) when (!(ex is AppointmentBaseException))
            {
                throw new DoctorNotFoundException(doctorName);
            }
        }


        //חיפוש תור לפי שם רופא והעיר שלו
        public async Task<List<AppointmentsSlot>> GetAvailableSlotsByProviderAndCityAsync(string doctorName, string cityName)
        {
            if (string.IsNullOrWhiteSpace(doctorName))
            {
                throw new InvalidAppointmentDataException("DoctorName");
            }
            if (string.IsNullOrWhiteSpace(cityName))
            {
                throw new InvalidAppointmentDataException("CityName");
            }
            try
            {
                var providerId = await _serviceProviderManagementDal.GetProviderKeyByName(doctorName);
                if (providerId <= 0)
                {
                    throw new DoctorNotFoundException(doctorName);
                }
                var allSlots = await _appointmentsSlotManagementDal.GetAppointmentSlotByCityAndServiceName(providerId, cityName);
                if (allSlots == null || allSlots.Count == 0)
                {
                    throw new NoAvailableSlotsException($"for doctor {doctorName} in city {cityName}");
                }
                var availableSlots = allSlots
                    .Where(a => !a.IsBooked && a.SlotDate >= DateOnly.FromDateTime(DateTime.Now))
                    .ToList();
                if (!availableSlots.Any())
                {
                    throw new NoAvailableSlotsException($"for doctor {doctorName} in city {cityName}");
                }
                return availableSlots;
            }
            catch (Exception ex) when (!(ex is AppointmentBaseException))
            {
                throw new DoctorNotFoundException(doctorName);
            }
        }



        //חיפוש תור לפי מקצוע
        public async Task<List<AppointmentsSlot>> GetAvailableSlotsByServiceAsync(int serviceId)
        {
            if (serviceId <= 0)
            {
                throw new InvalidAppointmentDataException("ServiceId");
            }

            try
            {
                var allSlots = await _appointmentsSlotManagementDal.GetAppointmentSlotByServiceType(serviceId);

                if (allSlots == null || allSlots.Count == 0)
                {
                    throw new ServiceNotFoundException(serviceId);
                }
                var availableSlots = allSlots
                    .Where(a => !a.IsBooked && a.SlotDate >= DateOnly.FromDateTime(DateTime.Now))
                    .ToList();

                if (!availableSlots.Any())
                {
                    throw new NoAvailableSlotsException($"for service Id {serviceId}");
                }

                return availableSlots;
            }
            catch (Exception ex) when (!(ex is AppointmentBaseException))
            {
                throw new ServiceNotFoundException(serviceId);
            }
        }

        // ביטול תור לפי מזהה
        public async Task CancelAppointmentAsync(int patientKey)
        {
            try
            {
                // פשוט מבטל את התור - אם המטופל רואה אותו, הוא קיים
                var success = await _appointmentManagement.DeleteAppointment(patientKey);
                if (!success)
                {
                    throw new DatabaseException("appointment cancellation");
                }
            }
            catch (Exception ex) when (!(ex is AppointmentBaseException))
            {
                throw new DatabaseException("appointment cancellation");
            }
        }


        // שליפת כל התורים של משתמש לפי שם או מזהה משתמש
        public async Task<List<Appointment>> GetAppointmentsByUserAsync(string patientName)
        {
            if (string.IsNullOrWhiteSpace(patientName))
            {
                throw new InvalidAppointmentDataException("PatientName");
            }

            try
            {
                var patientKey = await _patientsManagementDal.GetPatientIDByName(patientName);
                if (patientKey <= 0)
                {
                    throw new PatientNotFoundException(patientName);
                }

                var appointments = await _appointmentManagement.GetAppointmentsByPatientIdAsync(patientKey);

                // סינון רק תורים עתידיים
                var futureAppointments = appointments
                    .Where(a => a.Slot.SlotDate >= DateOnly.FromDateTime(DateTime.Now))
                    .OrderBy(a => a.Slot.SlotDate)
                    .ThenBy(a => a.Slot.SlotStart)
                    .ToList();

                return futureAppointments;
            }
            catch (Exception ex) when (!(ex is AppointmentBaseException))
            {
                throw new PatientNotFoundException(patientName);
            }
        }

        public async Task<List<Appointment>> GetAppointmentsByPatientIdAsync(int patientKey)
        {
            if (patientKey <= 0)
            {
                throw new InvalidAppointmentDataException("PatientKey");
            }

            try
            {
                var appointments = await _appointmentManagement.GetAppointmentsByPatientIdAsync(patientKey);

                // סינון רק תורים עתידיים
                var futureAppointments = appointments
                    .Where(a => a.Slot.SlotDate >= DateOnly.FromDateTime(DateTime.Now))
                    .OrderBy(a => a.Slot.SlotDate)
                    .ThenBy(a => a.Slot.SlotStart)
                    .ToList();

                return futureAppointments;
            }
            catch (Exception ex) when (!(ex is AppointmentBaseException))
            {
                throw new PatientNotFoundException(patientKey);
            }
        }

        //קביעת תור לא בטוחה שהיא עובדת



        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public async Task<bool> BookAppointmentAsync(int slotId, int patientKey)
        {

            await _semaphore.WaitAsync();

            try
            {
                var slot = await _appointmentsSlotManagementDal.GetSlotByIdAsync(slotId);
                if (slot == null)
                {
                    throw new SlotNotFoundException(slotId);
                }
                if (slot.IsBooked)
                {
                    throw new SlotAlreadyBookedException(slotId);
                }

                // בדיקה שהתור לא בעבר
                var slotDateTime = slot.SlotDate.ToDateTime(slot.SlotStart);
                // בדיקת חפיפה בזמנים
                var existingAppointments = await _appointmentManagement.GetAppointmentsByPatientIdAsync(patientKey);
                var conflictingAppointment = existingAppointments.FirstOrDefault(a =>
                    a.Slot.SlotDate == slot.SlotDate &&
                    ((a.Slot.SlotStart <= slot.SlotStart && a.Slot.SlotEnd > slot.SlotStart) ||
                     (a.Slot.SlotStart < slot.SlotEnd && a.Slot.SlotEnd >= slot.SlotEnd)));

                if (conflictingAppointment != null)
                {
                    throw new TimeConflictException(slotDateTime);
                }
                // בדיקה שהרופא פעיל
                if (!slot.ProviderKeyNavigation.IsActive)
                {
                    throw new DoctorNotActiveException(slot.ProviderKeyNavigation.Name);
                }

                // ביצוע הקביעה
                slot.IsBooked = true;
                var appointment = new Appointment
                {
                    SlotId = slotId,
                    PatientKey = patientKey
                };

                await _appointmentManagement.AddAppointment(appointment);
                return true;
            }
            catch (Exception ex) when (!(ex is AppointmentBaseException))
            {
                // שגיאה לא צפויה - הופכים אותה לדטאבייס אקסיפשן
                throw new DatabaseException("appointment booking");
            }
            finally
            {
                _semaphore.Release();
            }
        }


        //
        public async Task<bool> GenerateSlotsForProviderAsync(int providerKey, DateOnly startDate, DateOnly endDate)
        {
            if (startDate < DateOnly.FromDateTime(DateTime.Now))
            {
                throw new PastAppointmentException();
            }

            var maxDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(3));
            if (endDate > maxDate)
            {
                throw new InvalidAppointmentDataException("Cannot generate slots more than 3 months in advance");
            }

            try
            {
                var provider = await _serviceProviderManagementDal.GetProviderWithWorkHoursAsync(providerKey);
                if (provider == null)
                {
                    throw new DoctorNotFoundException(providerKey);
                }

                if (!provider.IsActive)
                {
                    throw new DoctorNotActiveException(provider.Name);
                }

                // בדיקה שיש שעות עבודה
                if (provider.WorkHours == null || !provider.WorkHours.Any())
                {
                    throw new InvalidAppointmentDataException($"No work hours defined for provider {provider.Name}");
                }

                // בדיקה שיש זמן פגישה תקין
                if (provider.MeetingTime <= 0 || provider.MeetingTime > 480) // מקסימום 8 שעות
                {
                    throw new InvalidAppointmentDataException($"Invalid meeting duration for provider {provider.Name}");
                }

                int slotsCreated = 0;

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

                    // אם אין שעות עבודה ביום זה - ממשיך ליום הבא
                    if (!workHours.Any())
                        continue;

                    // 4. עבור כל חלון זמן של עבודה - יצירת סלוטים
                    foreach (var workHour in workHours)
                    {
                        // בדיקת תקינות שעות העבודה
                        if (workHour.StartTime >= workHour.EndTime)
                        {
                            throw new InvalidAppointmentDataException($"Invalid work hours for provider {provider.Name} on weekday {weekday}");
                        }

                        var currentTime = workHour.StartTime;
                        var meetingDuration = provider.MeetingTime ?? 30; // משך פגישה בדקות, ברירת מחדל 30

                        while (currentTime.AddMinutes(meetingDuration) <= workHour.EndTime)
                        {
                            try
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
                                    slotsCreated++;

                                    // מגבלת בטיחות - לא יותר מ-10,000 סלוטים בפעם אחת
                                    if (slotsCreated > 10000)
                                    {
                                        throw new InvalidAppointmentDataException("Too many slots to create in single operation");
                                    }
                                }
                            }
                            catch (Exception ex) when (!(ex is AppointmentBaseException))
                            {
                                throw new DatabaseException($"slot creation for {date:yyyy-MM-dd} at {currentTime}");
                            }

                            currentTime = currentTime.AddMinutes(meetingDuration);
                        }
                    }
                }


                // אם לא נוצרו סלוטים בכלל
                if (slotsCreated == 0)
                {
                    throw new NoAvailableSlotsException($"for provider {provider.Name} in the specified date range");
                }

                return true;
            }
            catch (Exception ex) when (!(ex is AppointmentBaseException))
            {
                throw new DatabaseException("slot generation");
            }
        }

    }



}
