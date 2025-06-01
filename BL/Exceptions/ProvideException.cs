using System;

namespace BLL.Exceptions
{
    /// <summary>
    /// רופא לא נמצא
    /// </summary>
    public class DoctorNotFoundException : ClinicBaseException
    {
        public DoctorNotFoundException(string doctorName)
            : base($"The doctor with name '{doctorName}' does not exist in the system.", 404)
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

    /// <summary>
    /// רופא לא פעיל
    /// </summary>
    public class DoctorNotActiveException : ClinicBaseException
    {
        public DoctorNotActiveException(string doctorName)
            : base($"The doctor '{doctorName}' is currently not active and appointments cannot be booked.", 400)
        {
        }

        public DoctorNotActiveException()
            : base("The doctor is currently not active and appointments cannot be booked.", 400)
        {
        }
    }

    /// <summary>
    /// שירות לא נמצא
    /// </summary>
    public class ServiceNotFoundException : ClinicBaseException
    {
        public ServiceNotFoundException(int serviceId)
            : base($"The service with Id {serviceId} does not exist in the system.", 404)
        {
        }

        public ServiceNotFoundException(string serviceName)
            : base($"The service '{serviceName}' does not exist in the system.", 404)
        {
        }

        public ServiceNotFoundException()
            : base("The requested service does not exist in the system.", 404)
        {
        }
    }

    /// <summary>
    /// סניף לא נמצא
    /// </summary>
    public class BranchNotFoundException : ClinicBaseException
    {
        public BranchNotFoundException(int branchId)
            : base($"The branch with Id {branchId} does not exist in the system.", 404)
        {
        }

        public BranchNotFoundException(string branchName)
            : base($"The branch '{branchName}' does not exist in the system.", 404)
        {
        }

        public BranchNotFoundException()
            : base("The requested branch does not exist in the system.", 404)
        {
        }
    }

    /// <summary>
    /// עיר לא נמצאה
    /// </summary>
    public class CityNotFoundException : ClinicBaseException
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

    /// <summary>
    /// רחוב לא נמצא
    /// </summary>
    public class StreetNotFoundException : ClinicBaseException
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
}