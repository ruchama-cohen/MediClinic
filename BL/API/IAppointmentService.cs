using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.API
{
    public interface IAppointmentService
    {
        Task<List<Appointment>> GetAppointmentsByProviderNameAsync(string doctorName, string clinicName = null);

    }
}
