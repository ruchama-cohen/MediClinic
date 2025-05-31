namespace WebAPI.Services
{
    public interface IJwtService
    {
        string GenerateToken(int patientKey, string patientId, string patientName);
        bool ValidateToken(string token);
        int GetPatientKeyFromToken(string token);
    }
}