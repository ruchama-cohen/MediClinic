using BLL.API;
using BLL.Exceptions;
using DAL.API;

namespace BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly IPatientsManagement _patientsManagement;
        private readonly IPasswordService _passwordService;

        public AuthService(IPatientsManagement patientsManagement, IPasswordService passwordService)
        {
            _patientsManagement = patientsManagement;
            _passwordService = passwordService;
        }

        public async Task<int> Login(string id, string password)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new InvalidAppointmentDataException("Patient ID is required");

            if (string.IsNullOrWhiteSpace(password))
                throw new InvalidAppointmentDataException("Password is required");

            var patient = await _patientsManagement.GetPatientById(id);
            if (patient == null)
                return -1;

            if (string.IsNullOrEmpty(patient.PatientPassword))
                return -2;

            if (_passwordService.VerifyPassword(password, patient.PatientPassword))
            {
                return patient.PatientKey;
            }

            return -1;
        }

        public async Task<bool> SetPasswordForTesting(string patientId, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(patientId))
                throw new InvalidAppointmentDataException("Patient ID is required");

            if (string.IsNullOrWhiteSpace(newPassword))
                throw new InvalidAppointmentDataException("Password is required");

            if (newPassword.Length < 4 || newPassword.Length > 15)
                throw new InvalidAppointmentDataException("Password must be between 4 and 15 characters");

            var patient = await _patientsManagement.GetPatientById(patientId);
            if (patient == null)
                throw new PatientNotFoundException(patientId);

            string hashedPassword = _passwordService.HashPassword(newPassword);
            patient.PatientPassword = hashedPassword;

            bool updateResult = await _patientsManagement.UpdatePatient(patient);
            if (!updateResult)
                throw new DatabaseException("Failed to update patient password");

            return true;
        }
    }
}