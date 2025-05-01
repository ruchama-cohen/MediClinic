using BLL.API;
using DAL.Models;
using DAL.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentManagement _appointmentManagement;
        public AppointmentService(IAppointmentManagement appointmentManagement)
        {
            _appointmentManagement = appointmentManagement;
        }
        public async Task<List<Appointment>> GetAppointmentsByProviderNameAsync(string name)
        {
            var appointments = await _appointmentManagement.GetAppointmentsByProviderNameAsync(name);
        }
    }
}
