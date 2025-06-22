using BLL.Models;

namespace BLL.API
{
    public interface IPatientService
    {
        Task<bool> ChangePassword(string patientId, string oldPassword, string newPassword);
        Task<bool> UpdatePatientDetails(string patientId, string name, string email, string phone);
        Task<PatientModel?> GetPatientByIdString(string patientId);
        Task<bool> UpdatePatient(PatientModel model);
        Task<bool> UpdatePatientWithAddress(string patientId, string name, string email, string phone,
           string? cityName = null, string? streetName = null, int? houseNumber = null, string? postalCode = null);
    }
}