using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;
namespace DAL.API
{
    internal interface IPatientsManagement
    {
        Task AddPatient(Patient patient);
        Task<bool> UpdatePatient(Patient patient);
        Task<bool> DeletePatient(int id);
        Task<Patient> GetPatientById(int id);
        Task<List<Patient>> GetAllPatients();
        Task<string> GetCityByID(int id);

    }
}
