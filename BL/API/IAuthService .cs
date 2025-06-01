using BLL.Models;

namespace BLL.API
{
    public interface IAuthService
    {
        Task<int> Login(string id, string password);
        Task<bool> SetPasswordForTesting(string patientId, string newPassword); // הוספה זמנית לבדיקה
    }
}