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
            Console.WriteLine($"=== SET PASSWORD FOR TESTING DEBUG ===");
            Console.WriteLine($"1. SetPasswordForTesting called");
            Console.WriteLine($"2. PatientId: '{patientId}'");
            Console.WriteLine($"3. NewPassword: '{newPassword}'");

            try
            {
                Console.WriteLine($"4. Calling GetPatientById...");
                var patient = await _patientsManagement.GetPatientById(patientId);
                Console.WriteLine($"5. GetPatientById result: {(patient != null ? $"Found patient: {patient.PatientName} (Key: {patient.PatientKey})" : "NULL - Patient not found")}");

                if (patient == null)
                {
                    Console.WriteLine($"6. Patient is null, returning false");
                    return false;
                }

                Console.WriteLine($"7. Current patient password: '{patient.PatientPassword}' (length: {patient.PatientPassword?.Length ?? 0})");
                Console.WriteLine($"8. Hashing new password...");

                string hashedPassword = _passwordService.HashPassword(newPassword);
                Console.WriteLine($"9. Hashed password: '{hashedPassword}' (length: {hashedPassword?.Length ?? 0})");

                patient.PatientPassword = hashedPassword;
                Console.WriteLine($"10. Password set to patient object");

                Console.WriteLine($"11. Calling UpdatePatient...");
                bool updateResult = await _patientsManagement.UpdatePatient(patient);
                Console.WriteLine($"12. UpdatePatient result: {updateResult}");

                return updateResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"13. EXCEPTION in SetPasswordForTesting: {ex.GetType().Name}: {ex.Message}");
                Console.WriteLine($"14. Stack Trace: {ex.StackTrace}");
                return false;
            }
            finally
            {
                Console.WriteLine($"=== END SET PASSWORD FOR TESTING DEBUG ===");
            }
        }
    }
}
