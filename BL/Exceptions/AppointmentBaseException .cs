using System;

namespace BLL.Exceptions
{
    /// <summary>
    /// תור לא נמצא
    /// </summary>
    public class AppointmentNotFoundException : ClinicBaseException
    {
        public AppointmentNotFoundException(int appointmentId)
            : base($"The appointment with Id {appointmentId} does not exist in the system.", 404)
        {
        }

        public AppointmentNotFoundException()
            : base("The requested appointment does not exist in the system.", 404)
        {
        }
    }

    /// <summary>
    /// סלוט לא נמצא
    /// </summary>
    public class SlotNotFoundException : ClinicBaseException
    {
        public SlotNotFoundException(int slotId)
            : base($"The slot with Id {slotId} does not exist in the system.", 404)
        {
        }

        public SlotNotFoundException()
            : base("The requested slot does not exist in the system.", 404)
        {
        }
    }

    /// <summary>
    /// סלוט כבר תפוס
    /// </summary>
    public class SlotAlreadyBookedException : ClinicBaseException
    {
        public SlotAlreadyBookedException(int slotId)
            : base($"The slot with Id {slotId} is already booked by another patient.", 409)
        {
        }

        public SlotAlreadyBookedException()
            : base("This appointment slot is already booked by someone else.", 409)
        {
        }
    }

    /// <summary>
    /// חפיפה בזמנים
    /// </summary>
    public class TimeConflictException : ClinicBaseException
    {
        public TimeConflictException(DateTime conflictTime)
            : base($"You already have an appointment scheduled at {conflictTime:MM/dd/yyyy HH:mm}.", 409)
        {
        }

        public TimeConflictException()
            : base("You already have an appointment at this time, cannot book another appointment.", 409)
        {
        }
    }

    /// <summary>
    /// תור בעבר
    /// </summary>
    public class PastAppointmentException : ClinicBaseException
    {
        public PastAppointmentException(DateTime appointmentTime)
            : base($"Cannot book an appointment for {appointmentTime:MM/dd/yyyy HH:mm} as this time has already passed.", 400)
        {
        }

        public PastAppointmentException()
            : base("Cannot book an appointment for a time that has already passed.", 400)
        {
        }
    }

    /// <summary>
    /// מועד מוקדם מדי
    /// </summary>
    public class TooEarlyBookingException : ClinicBaseException
    {
        public TooEarlyBookingException(int minimumMinutes)
            : base($"Appointments must be booked at least {minimumMinutes} minutes in advance.", 400)
        {
        }

        public TooEarlyBookingException()
            : base("Appointments must be booked at least 30 minutes in advance.", 400)
        {
        }
    }

    /// <summary>
    /// אין תורים פנויים
    /// </summary>
    public class NoAvailableSlotsException : ClinicBaseException
    {
        public NoAvailableSlotsException(string criteria)
            : base($"No available appointment slots found {criteria} for the requested period.", 404)
        {
        }

        public NoAvailableSlotsException()
            : base("No available appointment slots found for the requested period.", 404)
        {
        }
    }

    /// <summary>
    /// חריגה ממגבלת תורים יומית
    /// </summary>
    public class DailyLimitExceededException : ClinicBaseException
    {
        public DailyLimitExceededException(int maxAppointments)
            : base($"You have reached the daily appointment limit of {maxAppointments} appointments per day.", 429)
        {
        }

        public DailyLimitExceededException()
            : base("You have reached the daily appointment limit.", 429)
        {
        }
    }


}