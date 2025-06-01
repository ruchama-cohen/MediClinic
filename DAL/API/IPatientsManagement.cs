using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;
namespace DAL.API
{
    public interface IPatientsManagement
    {
        Task AddPatient(Patient patient);
        Task<bool> UpdatePatient(Patient patient);
        Task<bool> DeletePatient(int id);
        Task<Patient> GetPatientById(string id);
        Task<Patient> GetPatientByIdString(int id);
        Task<List<Patient>> GetAllPatients();
        Task<string> GetCityByID(int id);
        Task<int> GetPatientIDByName(string name);

    }
}
