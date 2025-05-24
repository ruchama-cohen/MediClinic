using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.API
{
    public interface IAppointmentManagement
    {
        Task<bool> DeleteAppointment(int id);
        Task AddAppointment(Appointment appointment);
        Task<List<Appointment>> GetAppointmentsByPatientIdAsync(int patientKey);
    }
}
