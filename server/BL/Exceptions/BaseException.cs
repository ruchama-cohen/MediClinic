using System;

namespace BLL.Exceptions
{
    /// <summary>
    /// בסיס לכל השגיאות במערכת הקליניקה
    /// </summary>
    public abstract class ClinicBaseException : Exception
    {
        public int StatusCode { get; }

        protected ClinicBaseException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }

    /// <summary>
    /// שגיאת נתונים לא תקינים
    /// </summary>
    public class InvalidDataException : ClinicBaseException
    {
        public InvalidDataException(string fieldName)
            : base($"The field {fieldName} is invalid or missing.", 400)
        {
        }

        public InvalidDataException(string fieldName, string reason)
            : base($"The field {fieldName} is invalid: {reason}.", 400)
        {
        }

        public InvalidDataException()
            : base("One or more of the provided data is invalid.", 400)
        {
        }
    }

    /// <summary>
    /// שגיאת מסד נתונים
    /// </summary>
    public class DatabaseException : ClinicBaseException
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

    /// <summary>
    /// שגיאת הרשאות
    /// </summary>
    public class UnauthorizedException : ClinicBaseException
    {
        public UnauthorizedException(string action)
            : base($"You are not authorized to perform the action: {action}.", 403)
        {
        }

        public UnauthorizedException()
            : base("You are not authorized to perform this action.", 403)
        {
        }
    }

    /// <summary>
    /// שגיאת נתונים כפולים
    /// </summary>
    public class DuplicateDataException : ClinicBaseException
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