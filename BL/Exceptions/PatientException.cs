using System;

namespace BLL.Exceptions
{
    /// <summary>
    /// מטופל לא נמצא
    /// </summary>
    public class PatientNotFoundException : ClinicBaseException
    {
        public PatientNotFoundException(int patientKey)
            : base($"The patient with Id {patientKey} does not exist in the system.", 404)
        {
        }

        public PatientNotFoundException(string patientId)
            : base($"The patient with ID '{patientId}' does not exist in the system.", 404)
        {
        }

        public PatientNotFoundException()
            : base("The requested patient does not exist in the system.", 404)
        {
        }
    }

    /// <summary>
    /// שגיאת עדכון פרטי פציינט
    /// </summary>
    public class PatientUpdateException : ClinicBaseException
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

    /// <summary>
    /// שגיאת אימות סיסמה
    /// </summary>
    public class PasswordValidationException : ClinicBaseException
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

    /// <summary>
    /// כתובת לא תקינה
    /// </summary>
    public class AddressValidationException : ClinicBaseException
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

    /// <summary>
    /// כתובת לא נמצאה
    /// </summary>
    public class AddressNotFoundException : ClinicBaseException
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
}