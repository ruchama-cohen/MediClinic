using BLL.API;
using BLL.Models;
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
            try
            {
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
            catch (Exception)
            {
                return -1;
            }
        }

        public async Task<bool> SetPasswordForTesting(string patientId, string newPassword)
        {
            try
            {
                var patient = await _patientsManagement.GetPatientById(patientId);
                if (patient == null)
                    return false;

                patient.PatientPassword = _passwordService.HashPassword(newPassword);

                return await _patientsManagement.UpdatePatient(patient);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
