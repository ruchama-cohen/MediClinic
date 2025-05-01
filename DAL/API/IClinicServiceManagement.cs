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
        Task<bool> DeleteClinicService(string serviceName);

    }
}
