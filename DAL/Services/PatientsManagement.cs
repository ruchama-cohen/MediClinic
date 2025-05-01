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

        public async Task<string> GetCityByID(int id)
        {
            return await _context.Patients
                .Where(x => x.PatientId == id)
                .Select(x => x.Address.City)
                .FirstOrDefaultAsync();
        }

        public async Task<Patient> GetPatientById(int id)
        {
            return await _context.Patients.FirstOrDefaultAsync(p => p.PatientId == id);
        }


        public async Task<bool> UpdatePatient(Patient patient)
        {
            var patientN = await _context.Patients.FindAsync(patient.PatientId);

            if (patientN == null)
                return false;

            _context.Entry(patientN).CurrentValues.SetValues(patient);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
