using System;

namespace BLL.Exceptions
{
    /// <summary>
    /// שגיאת נתוני תור לא תקינים
    /// </summary>
    public class InvalidAppointmentDataException : ClinicBaseException
    {
        public InvalidAppointmentDataException(string message)
            : base($"Invalid appointment data: {message}", 400)
        {
        }

        public InvalidAppointmentDataException()
            : base("Invalid appointment data provided.", 400)
        {
        }
    }
}