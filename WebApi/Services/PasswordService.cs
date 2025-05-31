using BCrypt.Net;

namespace WebAPI.Services
{
    public class PasswordService : IPasswordService
    {
        public string HashPassword(string password)
        {
            return BCrypt.HashPassword(password, BCrypt.GenerateSalt());
        }

        public bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Verify(password, hash);
        }
    }
}