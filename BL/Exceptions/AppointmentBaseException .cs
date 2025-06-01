using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Exceptions
{
    public abstract class AppointmentBaseException : Exception
    {
        public int StatusCode { get; }

        protected AppointmentBaseException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }

    // תור לא נמצא
    public class AppointmentNotFoundException : AppointmentBaseException
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

    // סלוט כבר תפוס
    public class SlotAlreadyBookedException : AppointmentBaseException
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

    // מטופל לא נמצא
    public class PatientNotFoundException : AppointmentBaseException
    {
        public PatientNotFoundException(int patientKey)
            : base($"The patient with Id {patientKey} does not exist in the system.", 404)
        {
        }

        public PatientNotFoundException(string patientName)
            : base($"The patient with name {patientName} does not exist in the system.", 404)
        {
        }

        public PatientNotFoundException()
            : base("The requested patient does not exist in the system.", 404)
        {
        }
    }

    // רופא לא נמצא
    public class DoctorNotFoundException : AppointmentBaseException
    {
        public DoctorNotFoundException(string doctorName)
            : base($"The doctor with name {doctorName} does not exist in the system.", 404)
        {
        }

        public DoctorNotFoundException(int providerId)
            : base($"The doctor with Id {providerId} does not exist in the system.", 404)
        {
        }

        public DoctorNotFoundException()
            : base("The requested doctor does not exist in the system.", 404)
        {
        }
    }

    // רופא לא פעיל
    public class DoctorNotActiveException : AppointmentBaseException
    {
        public DoctorNotActiveException(string doctorName)
            : base($"The doctor {doctorName} is currently not active and appointments cannot be booked.", 400)
        {
        }

        public DoctorNotActiveException()
            : base("The doctor is currently not active and appointments cannot be booked.", 400)
        {
        }
    }

    // חפיפה בזמנים
    public class TimeConflictException : AppointmentBaseException
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

    // תור בעבר
    public class PastAppointmentException : AppointmentBaseException
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

    // מועד מוקדם מדי
    public class TooEarlyBookingException : AppointmentBaseException
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

    // חריגה ממגבלת תורים יומית
    public class DailyLimitExceededException : AppointmentBaseException
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

    // סניף לא נמצא
    public class BranchNotFoundException : AppointmentBaseException
    {
        public BranchNotFoundException(int branchId)
            : base($"The branch with Id {branchId} does not exist in the system.", 404)
        {
        }

        public BranchNotFoundException(string branchName)
            : base($"The branch with name {branchName} does not exist in the system.", 404)
        {
        }

        public BranchNotFoundException()
            : base("The requested branch does not exist in the system.", 404)
        {
        }
    }

    // שירות לא נמצא
    public class ServiceNotFoundException : AppointmentBaseException
    {
        public ServiceNotFoundException(int serviceId)
            : base($"The service with Id {serviceId} does not exist in the system.", 404)
        {
        }

        public ServiceNotFoundException(string serviceName)
            : base($"The service {serviceName} does not exist in the system.", 404)
        {
        }

        public ServiceNotFoundException()
            : base("The requested service does not exist in the system.", 404)
        {
        }
    }

    // נתונים לא תקינים
    public class InvalidAppointmentDataException : AppointmentBaseException
    {
        public InvalidAppointmentDataException(string fieldName)
            : base($"The field {fieldName} is invalid or missing.", 400)
        {
        }

        public InvalidAppointmentDataException()
            : base("One or more of the provided data is invalid.", 400)
        {
        }
    }

    // אין תורים פנויים
    public class NoAvailableSlotsException : AppointmentBaseException
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

    // שגיאת רשאות
    public class UnauthorizedAppointmentException : AppointmentBaseException
    {
        public UnauthorizedAppointmentException(string action)
            : base($"You are not authorized to perform the action: {action}.", 403)
        {
        }

        public UnauthorizedAppointmentException()
            : base("You are not authorized to perform this action.", 403)
        {
        }
    }

    // סלוט לא נמצא
    public class SlotNotFoundException : AppointmentBaseException
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

    // שגיאת מסד נתונים  
    public class DatabaseException : AppointmentBaseException
    {
        public DatabaseException(string operation)
            : base($"Database error occurred while performing operation: {operation}.", 500)
        {
        }

        public DatabaseException()
            : base("Database error occurred, please try again later.", 500)
        {
        }
    }

    public class CityNotFoundException : AppointmentBaseException
    {
        public CityNotFoundException(string cityName)
            : base($"The city '{cityName}' does not exist in the system.", 404)
        {
        }

        public CityNotFoundException(int cityId)
            : base($"The city with Id {cityId} does not exist in the system.", 404)
        {
        }

        public CityNotFoundException()
            : base("The requested city does not exist in the system.", 404)
        {
        }
    }

    // רחוב לא נמצא
    public class StreetNotFoundException : AppointmentBaseException
    {
        public StreetNotFoundException(string streetName, string cityName)
            : base($"The street '{streetName}' in city '{cityName}' does not exist in the system.", 404)
        {
        }

        public StreetNotFoundException(int streetId)
            : base($"The street with Id {streetId} does not exist in the system.", 404)
        {
        }

        public StreetNotFoundException()
            : base("The requested street does not exist in the system.", 404)
        {
        }
    }

    // כתובת לא תקינה
    public class AddressValidationException : AppointmentBaseException
    {
        public AddressValidationException(string field)
            : base($"Address validation failed: {field} is invalid.", 400)
        {
        }

        public AddressValidationException()
            : base("Address validation failed.", 400)
        {
        }
    }

    // כתובת לא נמצאה
    public class AddressNotFoundException : AppointmentBaseException
    {
        public AddressNotFoundException(int addressId)
            : base($"The address with Id {addressId} does not exist in the system.", 404)
        {
        }

        public AddressNotFoundException()
            : base("The requested address does not exist in the system.", 404)
        {
        }
    }

    // שגיאת עדכון פרטי פציינט
    public class PatientUpdateException : AppointmentBaseException
    {
        public PatientUpdateException(string reason)
            : base($"Failed to update patient details: {reason}", 400)
        {
        }

        public PatientUpdateException()
            : base("Failed to update patient details.", 400)
        {
        }
    }

    // שגיאת אימות סיסמה
    public class PasswordValidationException : AppointmentBaseException
    {
        public PasswordValidationException(string reason)
            : base($"Password validation failed: {reason}", 400)
        {
        }

        public PasswordValidationException()
            : base("Password validation failed.", 400)
        {
        }
    }

    // שגיאת רשאות - משתמש לא מורשה לבצע פעולה
    public class InsufficientPermissionsException : AppointmentBaseException
    {
        public InsufficientPermissionsException(string action)
            : base($"Insufficient permissions to perform action: {action}", 403)
        {
        }

        public InsufficientPermissionsException()
            : base("Insufficient permissions to perform this action.", 403)
        {
        }
    }

    // נתונים כפולים
    public class DuplicateDataException : AppointmentBaseException
    {
        public DuplicateDataException(string entityType, string value)
            : base($"A {entityType} with the value '{value}' already exists in the system.", 409)
        {
        }

        public DuplicateDataException()
            : base("Duplicate data found in the system.", 409)
        {
        }
    }
}
