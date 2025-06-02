using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.API;
using DAL.API;
using DAL.Models;

namespace BLL.Services
{
    public class ClinicServiceService : IClinicServiceService  // שם שונה!
    {
        private readonly IClinicServiceManagement _clinicServiceManagementDal;

        public ClinicServiceService(IClinicServiceManagement clinicServiceManagementDal)
        {
            _clinicServiceManagementDal = clinicServiceManagementDal;
        }

        public async Task<List<DAL.Models.ClinicService>> GetAllClinicServicesAsync()  
        {
            var allServices = await _clinicServiceManagementDal.GetAllServices();
            return allServices;
        }
    }
}