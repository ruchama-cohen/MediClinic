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
        Task<List<ClinicService>> GetAllServices();
    
        Task<bool> UpdateService(ClinicService service);
        Task<bool> ServiceExists(int serviceId);
    }
}