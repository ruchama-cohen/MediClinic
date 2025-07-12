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
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public AppointmentService(IAppointmentManagement appointmentManagement, IAppointmentsSlotManagement appointmentsSlotManagementDal, IServiceProviderManagement serviceProviderManagementDal, IPatientsManagement patientsManagementDal)
        {
            _serviceProviderManagementDal = serviceProviderManagementDal;
            _appointmentManagement = appointmentManagement;
            _appointmentsSlotManagementDal = appointmentsSlotManagementDal;
            _patientsManagementDal = patientsManagementDal;
        }

        public async Task<List<AppointmentsSlot>> GetAppointmentsByProviderNameAsync(string doctorName)
        {
            if (string.IsNullOrWhiteSpace(doctorName))
                throw new InvalidAppointmentDataException("Doctor name is required");

            var providerId = await _serviceProviderManagementDal.GetProviderKeyByName(doctorName);
            if (providerId <= 0)
                throw new DoctorNotFoundException(doctorName);

            var appointments = await _appointmentsSlotManagementDal.GetAppointmentsSlotsByServiceProviderID(providerId);
            if (appointments == null || appointments.Count == 0)
                throw new NoAvailableSlotsException($"for doctor {doctorName}");

            var availableSlots = appointments
                .Where(a => !a.IsBooked && a.SlotDate >= DateOnly.FromDateTime(DateTime.Now))
                .ToList();

            if (!availableSlots.Any())
                throw new NoAvailableSlotsException($"for doctor {doctorName}");

            return availableSlots;
        }

        public async Task<List<AppointmentsSlot>> GetAvailableSlotsByProviderAndCityAsync(string doctorName, string cityName)
        {
            if (string.IsNullOrWhiteSpace(doctorName))
                throw new InvalidAppointmentDataException("Doctor name is required");

            if (string.IsNullOrWhiteSpace(cityName))
                throw new InvalidAppointmentDataException("City name is required");

            var providerId = await _serviceProviderManagementDal.GetProviderKeyByName(doctorName);
            if (providerId <= 0)
                throw new DoctorNotFoundException(doctorName);

            var provider = await _serviceProviderManagementDal.GetProviderWithWorkHoursAsync(providerId);
            if (provider == null)
                throw new DoctorNotFoundException(doctorName);

            var allSlots = await _appointmentsSlotManagementDal.GetAppointmentSlotByCityAndServiceName(provider.ServiceId, cityName);
            if (allSlots == null || allSlots.Count == 0)
                throw new NoAvailableSlotsException($"for doctor {doctorName} in city {cityName}");

            var doctorSlots = allSlots
                .Where(a => a.ProviderKey == providerId)
                .ToList();

            if (!doctorSlots.Any())
                throw new NoAvailableSlotsException($"for doctor {doctorName} in city {cityName}");

            return doctorSlots;
        }

        public async Task<List<AppointmentsSlot>> GetAvailableSlotsByServiceAsync(int serviceId)
        {
            if (serviceId <= 0)
                throw new InvalidAppointmentDataException("Service ID must be positive");

            var allSlots = await _appointmentsSlotManagementDal.GetAppointmentSlotByServiceType(serviceId);
            if (allSlots == null || allSlots.Count == 0)
                throw new ServiceNotFoundException(serviceId);

            var availableSlots = allSlots
                .Where(a => !a.IsBooked && a.SlotDate >= DateOnly.FromDateTime(DateTime.Now))
                .ToList();

            if (!availableSlots.Any())
                throw new NoAvailableSlotsException($"for service Id {serviceId}");

            return availableSlots;
        }

        // מתודה מעודכנת לעבוד עם PatientKey כמחרוזת (מהקונטרולר)
        public async Task<bool> BookAppointmentAsync(int slotId, string patientKeyAsString)
        {
            await _semaphore.WaitAsync();

            try
            {
                if (slotId <= 0)
                    throw new InvalidAppointmentDataException("Slot ID must be positive");

                if (string.IsNullOrWhiteSpace(patientKeyAsString))
                    throw new InvalidAppointmentDataException("Patient Key is required");

                // המרה מ-string ל-int
                if (!int.TryParse(patientKeyAsString, out int patientKey) || patientKey <= 0)
                    throw new InvalidAppointmentDataException("Valid Patient Key is required");

                var patient = await _patientsManagementDal.GetPatientByIdString(patientKey);
                if (patient == null)
                    throw new PatientNotFoundException(patientKeyAsString);

                var slot = await _appointmentsSlotManagementDal.GetSlotByIdAsync(slotId);
                if (slot == null)
                    throw new SlotNotFoundException(slotId);

                if (slot.IsBooked)
                    throw new SlotAlreadyBookedException(slotId);

                var slotDateTime = slot.SlotDate.ToDateTime(slot.SlotStart);
                if (slotDateTime <= DateTime.Now.AddMinutes(30))
                    throw new PastAppointmentException(slotDateTime);

                var existingAppointments = await _appointmentManagement.GetAppointmentsByPatientIdAsync(patient.PatientKey);
                var conflictingAppointment = existingAppointments.FirstOrDefault(a =>
                    a.Slot.SlotDate == slot.SlotDate &&
                    ((a.Slot.SlotStart <= slot.SlotStart && a.Slot.SlotEnd > slot.SlotStart) ||
                     (a.Slot.SlotStart < slot.SlotEnd && a.Slot.SlotEnd >= slot.SlotEnd)));

                if (conflictingAppointment != null)
                    throw new TimeConflictException(slotDateTime);

                if (slot.ProviderKeyNavigation != null && !slot.ProviderKeyNavigation.IsActive)
                    throw new DoctorNotActiveException(slot.ProviderKeyNavigation.Name);

                var appointment = new Appointment
                {
                    SlotId = slotId,
                    PatientKey = patient.PatientKey
                };

                await _appointmentManagement.AddAppointment(appointment);
                await _appointmentsSlotManagementDal.UpdateQueueAvailability(slotId, true);

                return true;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task CancelAppointmentAsync(int appointmentId)
        {
            if (appointmentId <= 0)
                throw new InvalidAppointmentDataException("Appointment ID must be positive");

            var appointment = await _appointmentManagement.GetAppointmentByIdAsync(appointmentId);
            if (appointment == null)
                throw new AppointmentNotFoundException(appointmentId);

            var success = await _appointmentManagement.DeleteAppointment(appointmentId);
            if (!success)
                throw new DatabaseException("Failed to cancel appointment");

            await _appointmentsSlotManagementDal.UpdateQueueAvailability(appointment.SlotId, false);
        }

        public async Task<List<Appointment>> GetAppointmentsByUserAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new InvalidAppointmentDataException("Patient ID is required");

            var patient = await _patientsManagementDal.GetPatientById(id);

            if (patient == null)
                throw new PatientNotFoundException(id);

            if (patient.PatientKey <= 0)
                throw new PatientNotFoundException(id);

            var appointments = await _appointmentManagement.GetAppointmentsByPatientIdAsync(patient.PatientKey);

            var futureAppointments = appointments
                .Where(a => a.Slot.SlotDate >= DateOnly.FromDateTime(DateTime.Now))
                .OrderBy(a => a.Slot.SlotDate)
                .ThenBy(a => a.Slot.SlotStart)
                .ToList();

            return futureAppointments;
        }

        public async Task<List<Appointment>> GetAppointmentsByPatientIdAsync(int patientKey)
        {
            if (patientKey <= 0)
                throw new InvalidAppointmentDataException("Patient key must be positive");

            var appointments = await _appointmentManagement.GetAppointmentsByPatientIdAsync(patientKey);
            if (appointments == null)
                appointments = new List<Appointment>();

            var futureAppointments = appointments
                .Where(a => a.Slot.SlotDate >= DateOnly.FromDateTime(DateTime.Now))
                .OrderBy(a => a.Slot.SlotDate)
                .ThenBy(a => a.Slot.SlotStart)
                .ToList();

            return futureAppointments;
        }

        public async Task<bool> GenerateSlotsForProviderAsync(int providerKey, DateOnly startDate, DateOnly endDate)
        {
            if (providerKey <= 0)
                throw new InvalidAppointmentDataException("Provider key must be positive");

            if (startDate < DateOnly.FromDateTime(DateTime.Now))
                throw new PastAppointmentException();

            var maxDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(3));
            if (endDate > maxDate)
                throw new InvalidAppointmentDataException("Cannot generate slots more than 3 months in advance");

            var provider = await _serviceProviderManagementDal.GetProviderWithWorkHoursAsync(providerKey);
            if (provider == null)
                throw new DoctorNotFoundException(providerKey);

            if (!provider.IsActive)
                throw new DoctorNotActiveException(provider.Name);

            if (provider.WorkHours == null || !provider.WorkHours.Any())
                throw new InvalidAppointmentDataException($"No work hours defined for provider {provider.Name}");

            if (provider.MeetingTime <= 0 || provider.MeetingTime > 480)
                throw new InvalidAppointmentDataException($"Invalid meeting duration for provider {provider.Name}");

            int slotsCreated = 0;

            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                var weekday = (int)date.DayOfWeek;
                if (weekday == 0) weekday = 7;

                var workHours = provider.WorkHours
                    .Where(wh => wh.Weekday == weekday)
                    .ToList();

                if (!workHours.Any())
                    continue;

                foreach (var workHour in workHours)
                {
                    if (workHour.StartTime >= workHour.EndTime)
                        throw new InvalidAppointmentDataException($"Invalid work hours for provider {provider.Name} on weekday {weekday}");

                    var currentTime = workHour.StartTime;
                    var meetingDuration = provider.MeetingTime ?? 30;

                    while (currentTime.AddMinutes(meetingDuration) <= workHour.EndTime)
                    {
                        bool exists = await _appointmentsSlotManagementDal.AnySlotExistsAsync(providerKey, date, currentTime, provider.BranchId);
                        if (!exists)
                        {
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

                            if (slotsCreated > 10000)
                                throw new InvalidAppointmentDataException("Too many slots to create in single operation");
                        }

                        currentTime = currentTime.AddMinutes(meetingDuration);
                    }
                }
            }

            if (slotsCreated == 0)
                throw new NoAvailableSlotsException($"for provider {provider.Name} in the specified date range");

            return true;
        }

        public Task<bool> BookAppointmentAsync(int slotId, int patientKey)
        {
            throw new NotImplementedException();
        }
    }
}