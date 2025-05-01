using BLL.API;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    internal class AppointmentService : IAppointmentService
    {
        public Task<List<Appointment>> GetAppointmentsByProviderNameAsync(string name)
        {
            throw new NotImplementedException();
        }
    }
}
