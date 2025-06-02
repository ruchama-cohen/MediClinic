using DAL.Models;

namespace BLL.API
{
    public interface IClinicServiceService
    {
        Task<List<ClinicService>> GetAllClinicServicesAsync();
    }
}