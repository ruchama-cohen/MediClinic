using DAL.API;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Services
{
    public class PatientsManagement : IPatientsManagement
    {
        private readonly DB_Manager _context;

        public PatientsManagement(DB_Manager context)
        {
            _context = context;
        }
        public async Task AddPatient(Patient patient)
        {
            await _context.Patients.AddAsync(patient);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeletePatient(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient != null)
            {
                _context.Patients.Remove(patient);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<Patient>> GetAllPatients()
        {
            return await _context.Patients.ToListAsync();
        }


        public async Task<string?> GetCityByID(int id)
        {
            return await _context.Patients
                .Where(x => x.PatientKey == id && x.Address != null)
                .Select(x => x.Address.City.Name)
                .FirstOrDefaultAsync();
        }
        

        public async Task<Patient> GetPatientByIdString(int id)
        {
            return await _context.Patients.FirstOrDefaultAsync(p => p.PatientKey == id);
        }

        public async Task<Patient> GetPatientById(string id)
        {
            return await _context.Patients.FirstOrDefaultAsync(p => p.PatientId == id);
        }

        public async Task<bool> UpdatePatient(Patient updatedPatient)
        {
            var patientInDb = await _context.Patients.FindAsync(updatedPatient.PatientId);
            if (patientInDb == null)
                return false;

            _context.Entry(patientInDb).CurrentValues.SetValues(updatedPatient);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<int> GetPatientIDByName(string name)
        {
            var Patient = await _context.Patients
                           .FirstOrDefaultAsync(sp => sp.PatientName == name);

            return Patient.PatientKey;
        }

       
    }
}
