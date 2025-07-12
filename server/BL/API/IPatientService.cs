using BLL.Models;

namespace BLL.API
{
    public interface IPatientService
    {
        Task<bool> ChangePassword(string patientId, string oldPassword, string newPassword);
        Task<bool> ChangePasswordByKey(int patientKey, string oldPassword, string newPassword); // הוסף
        Task<bool> UpdatePatientDetails(string patientId, string name, string email, string phone);
        Task<bool> UpdatePatientDetailsByKey(int patientKey, string name, string email, string phone); // הוסף
        Task<PatientModel?> GetPatientByIdString(string patientId);
        Task<PatientModel?> GetPatientByKey(int patientKey);
        Task<bool> UpdatePatient(PatientModel model);
        Task<bool> UpdatePatientWithAddress(string patientId, string name, string email, string phone,
           string? cityName = null, string? streetName = null, int? houseNumber = null, string? postalCode = null);
        Task<bool> UpdatePatientWithAddress(int patientKey, string name, string email, string phone,
           string? cityName = null, string? streetName = null, int? houseNumber = null, string? postalCode = null); // הוסף
    }
}