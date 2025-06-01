using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.API;
using BLL.Models;
using DAL.API;
using DAL.Models;

namespace BLL.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientsManagement _patientManagement;
        private readonly IAddressManagement _addressManagement;
        private readonly IPasswordService _passwordService;
        private readonly AddressService _addressService;

        public PatientService(IPatientsManagement patientManagement, IAddressManagement addressManagement, IPasswordService passwordService)
        {
            _patientManagement = patientManagement;
            _addressManagement = addressManagement;
            _passwordService = passwordService;
            _addressService = new AddressService(patientManagement, addressManagement);
        }

        public async Task<bool> ChangePassword(string patientId, string oldPassword, string newPassword)
        {
            try
            {
                var patient = await _patientManagement.GetPatientById(patientId);
                if (patient == null)
                    return false;
                if (!_passwordService.VerifyPassword(oldPassword, patient.PatientPassword))
                    return false;
                patient.PatientPassword = _passwordService.HashPassword(newPassword);

                return await _patientManagement.UpdatePatient(patient);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdatePatientDetails(string patientId, string name, string email, string phone)
        {
            try
            {
                var existingPatient = await _patientManagement.GetPatientById(patientId);
                if (existingPatient == null)
                    return false;

                existingPatient.PatientName = name;
                existingPatient.Email = email;
                existingPatient.Phone = phone;

                return await _patientManagement.UpdatePatient(existingPatient);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<PatientModel?> GetPatientByIdString(string patientId)
        {
            try
            {
                var patient = await _patientManagement.GetPatientById(patientId);
                if (patient == null)
                    return null;

                return new PatientModel
                {
                    PatientKey = patient.PatientKey,
                    PatientId = patient.PatientId,
                    PatientName = patient.PatientName,
                    Email = patient.Email,
                    Phone = patient.Phone,
                    PatientPassword = "", 
                    address = patient.Address
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdatePatient(PatientModel model)
        {
            var existingPatient = await _patientManagement.GetPatientById(model.PatientId);
            if (existingPatient == null)
                return false;

            var addressId = await _addressService.GetOrAddAddressAsync(model.address);

            var updatedEntity = new Patient
            {
                PatientKey = model.PatientKey,
                PatientId = model.PatientId,
                PatientName = model.PatientName,
                Email = model.Email,
                Phone = model.Phone,
                PatientPassword = model.PatientPassword,
                AddressId = addressId,
                BirthDate = existingPatient.BirthDate,
                Gender = existingPatient.Gender,
                Address = model.address,
            };

            return await _patientManagement.UpdatePatient(updatedEntity);
        }

        public async Task<bool> UpdatePatientWithAddress(string patientId, string name, string email, string phone,
    string? cityName = null, string? streetName = null, int? houseNumber = null, string? postalCode = null)
        {
            try
            {
                var existingPatient = await _patientManagement.GetPatientById(patientId);
                if (existingPatient == null)
                    return false;
                existingPatient.PatientName = name;
                existingPatient.Email = email;
                existingPatient.Phone = phone;
                if (!string.IsNullOrWhiteSpace(cityName) && !string.IsNullOrWhiteSpace(streetName)
                    && houseNumber.HasValue && !string.IsNullOrWhiteSpace(postalCode))
                {
                    var address = new Address
                    {
                        City = new City { Name = cityName },
                        Street = new Street { Name = streetName },
                        HouseNumber = houseNumber.Value,
                        PostalCode = postalCode
                    };

                    var addressId = await _addressService.GetOrAddAddressAsync(address);
                    existingPatient.AddressId = addressId;
                }

                return await _patientManagement.UpdatePatient(existingPatient);
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
