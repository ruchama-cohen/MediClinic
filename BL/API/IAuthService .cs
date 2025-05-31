using BLL.Models;

namespace BLL.API
{
    public interface IAuthService
    {
        Task<int> Login(int id, string password);
        Task<bool> SetPasswordForTesting(int patientId, string newPassword); // הוספה זמנית לבדיקה
    }
}