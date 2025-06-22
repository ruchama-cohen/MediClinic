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
            Console.WriteLine($"GetPatientById called with: '{id}', Length: {id?.Length ?? 0}");

            try
            {
                var patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientId == id);
                Console.WriteLine($"Query result: {(patient != null ? $"Found patient {patient.PatientName}" : "No patient found")}");
                var allPatients = await _context.Patients.Take(3).ToListAsync();
                Console.WriteLine("First 3 patients in DB:");
                foreach (var p in allPatients)
                {
                    Console.WriteLine($"  PatientKey: {p.PatientKey}, PatientId: '{p.PatientId}', Name: {p.PatientName}");
                }

                return patient;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetPatientById: {ex.Message}");
                throw;
            }
        }

       
        public async Task<bool> UpdatePatient(Patient updatedPatient)
        {
            Console.WriteLine($"=== UPDATE PATIENT DEBUG ===");
            Console.WriteLine($"1. UpdatePatient called for PatientKey: {updatedPatient.PatientKey}");
            Console.WriteLine($"2. PatientId: '{updatedPatient.PatientId}'");
            Console.WriteLine($"3. PatientName: '{updatedPatient.PatientName}'");

            try
            {
                Console.WriteLine($"4. Finding patient by PatientKey: {updatedPatient.PatientKey}");
                var patientInDb = await _context.Patients.FindAsync(updatedPatient.PatientKey);

                if (patientInDb == null)
                {
                    Console.WriteLine($"5. Patient not found in database");
                    return false;
                }

                Console.WriteLine($"6. Found patient in DB: {patientInDb.PatientName}");
                Console.WriteLine($"7. Updating patient values...");

                _context.Entry(patientInDb).CurrentValues.SetValues(updatedPatient);

                Console.WriteLine($"8. Saving changes...");
                await _context.SaveChangesAsync();

                Console.WriteLine($"9. Patient updated successfully!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"10. EXCEPTION in UpdatePatient: {ex.GetType().Name}: {ex.Message}");
                Console.WriteLine($"11. Stack Trace: {ex.StackTrace}");
                return false;
            }
            finally
            {
                Console.WriteLine($"=== END UPDATE PATIENT DEBUG ===");
            }
        }


        public async Task<int> GetPatientIDByName(string name)
        {
            var Patient = await _context.Patients
                           .FirstOrDefaultAsync(sp => sp.PatientName == name);

            return Patient.PatientKey;
        }

       
    }
}
