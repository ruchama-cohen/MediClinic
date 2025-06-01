using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.API
{
    public interface IClinicServiceManagement
    {
        Task AddClinicService(ClinicService clinicService);
        Task<bool> DeleteClinicService(int serviceID);
        Task<List<ClinicService>> GetAllServices();
        Task<ClinicService?> GetServiceById(int serviceId);
        Task<ClinicService?> GetServiceByName(string serviceName);
        Task<bool> UpdateService(ClinicService service);
        Task<bool> ServiceExists(int serviceId);
    }
}