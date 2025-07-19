
using BLL.Models;

namespace BLL.API
{
    public interface IPatientService
    {
        Task<bool> ChangePassword(int patientKey, string oldPassword, string newPassword);
        Task<PatientModel?> GetPatientByKey(int patientKey);
        Task<bool> UpdatePatientPartial(int patientKey, string? name = null, string? email = null, string? phone = null,
            int? cityId = null, int? streetId = null, int? houseNumber = null, string? postalCode = null);
       
        // הסרנו פונקציות לא נחוצות
    }
}