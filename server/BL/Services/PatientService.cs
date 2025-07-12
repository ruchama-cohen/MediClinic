using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.API;
using BLL.Models;
using BLL.Exceptions;
using DAL.API;
using DAL.Models;

namespace BLL.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientsManagement _patientManagement;
        private readonly IAddressManagement _addressManagement;
        private readonly IPasswordService _passwordService;

        public PatientService(IPatientsManagement patientManagement, IAddressManagement addressManagement, IPasswordService passwordService)
        {
            _patientManagement = patientManagement;
            _addressManagement = addressManagement;
            _passwordService = passwordService;
        }

        public async Task<bool> ChangePassword(string patientId, string oldPassword, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(patientId))
                throw new InvalidAppointmentDataException("Patient ID is required");

            if (string.IsNullOrWhiteSpace(oldPassword))
                throw new InvalidAppointmentDataException("Current password is required");

            if (string.IsNullOrWhiteSpace(newPassword))
                throw new InvalidAppointmentDataException("New password is required");

            if (newPassword.Length < 4 || newPassword.Length > 15)
                throw new InvalidAppointmentDataException("Password must be between 4 and 15 characters");
            var patient = await _patientManagement.GetPatientById(patientId);
            if (patient == null)
                throw new PatientNotFoundException(patientId);

            if (string.IsNullOrEmpty(patient.PatientPassword))
                throw new InvalidAppointmentDataException("No password set for this patient");
            if (!_passwordService.VerifyPassword(oldPassword, patient.PatientPassword))
                throw new InvalidAppointmentDataException("Current password is incorrect");
            patient.PatientPassword = _passwordService.HashPassword(newPassword);
            bool result = await _patientManagement.UpdatePatient(patient);

            if (!result)
                throw new DatabaseException("Failed to update password");

            return true;
        }

        public async Task<bool> UpdatePatientDetails(string patientId, string name, string email, string phone)
        {
            if (string.IsNullOrWhiteSpace(patientId))
                throw new InvalidAppointmentDataException("Patient ID is required");

            if (string.IsNullOrWhiteSpace(name) || name.Length < 2)
                throw new InvalidAppointmentDataException("Name must be at least 2 characters");

            if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
                throw new InvalidAppointmentDataException("Valid email is required");

            if (string.IsNullOrWhiteSpace(phone) || phone.Length < 10)
                throw new InvalidAppointmentDataException("Valid phone number is required");
            var existingPatient = await _patientManagement.GetPatientById(patientId);
            if (existingPatient == null)
                throw new PatientNotFoundException(patientId);
            existingPatient.PatientName = name.Trim();
            existingPatient.Email = email.Trim();
            existingPatient.Phone = phone.Trim();

            bool result = await _patientManagement.UpdatePatient(existingPatient);
            if (!result)
                throw new DatabaseException("Failed to update patient details");

            return true;
        }

        public async Task<PatientModel?> GetPatientByIdString(string patientId)
        {
            if (string.IsNullOrWhiteSpace(patientId))
                throw new InvalidAppointmentDataException("Patient ID is required");

            var patient = await _patientManagement.GetPatientById(patientId);
            if (patient == null)
                throw new PatientNotFoundException(patientId);
            Address? address = null;
            if (patient.AddressId > 0)
            {
                address = await _addressManagement.GetAddressById(patient.AddressId);
            }

            return new PatientModel
            {
                PatientKey = patient.PatientKey,
                PatientId = patient.PatientId,
                PatientName = patient.PatientName,
                Email = patient.Email,
                Phone = patient.Phone,
                PatientPassword = "",
                address = address
            };
        }

        public async Task<PatientModel?> GetPatientByKey(int patientKey)
        {
            var patient = await _patientManagement.GetPatientByIdString(patientKey);

            if (patient == null)
                throw new PatientNotFoundException(patientKey.ToString());

            Address? address = null;
            if (patient.AddressId > 0)
            {
                address = await _addressManagement.GetAddressById(patient.AddressId);
            }

            return new PatientModel
            {
                PatientKey = patient.PatientKey,
                PatientId = patient.PatientId,
                PatientName = patient.PatientName,
                Email = patient.Email,
                Phone = patient.Phone,
                PatientPassword = "",
                address = address
            };
        }

        public async Task<bool> UpdatePatient(PatientModel model)
        {
            if (model == null)
                throw new InvalidAppointmentDataException("Patient model cannot be null");

            var existingPatient = await _patientManagement.GetPatientById(model.PatientId);
            if (existingPatient == null)
                throw new PatientNotFoundException(model.PatientId);
            int addressId = existingPatient.AddressId;
            if (model.address != null && model.address.City != null && model.address.Street != null)
            {
                addressId = await _addressManagement.CreateFullAddressAsync(
                    model.address.City.Name,
                    model.address.Street.Name,
                    model.address.HouseNumber,
                    model.address.PostalCode
                );
            }

            var updatedEntity = new Patient
            {
                PatientKey = model.PatientKey,
                PatientId = model.PatientId,
                PatientName = model.PatientName?.Trim() ?? existingPatient.PatientName,
                Email = model.Email?.Trim() ?? existingPatient.Email,
                Phone = model.Phone?.Trim() ?? existingPatient.Phone,
                PatientPassword = string.IsNullOrEmpty(model.PatientPassword) ? existingPatient.PatientPassword : model.PatientPassword,
                AddressId = addressId,
                BirthDate = existingPatient.BirthDate,
                Gender = existingPatient.Gender
            };

            bool result = await _patientManagement.UpdatePatient(updatedEntity);
            if (!result)
                throw new DatabaseException("Failed to update patient");

            return true;
        }

        public async Task<bool> UpdatePatientWithAddress(string patientId, string name, string email, string phone,
            string? cityName = null, string? streetName = null, int? houseNumber = null, string? postalCode = null)
        {
            if (string.IsNullOrWhiteSpace(patientId))
                throw new InvalidAppointmentDataException("Patient ID is required");

            var existingPatient = await _patientManagement.GetPatientById(patientId);
            if (existingPatient == null)
                throw new PatientNotFoundException(patientId);
            existingPatient.PatientName = name?.Trim() ?? existingPatient.PatientName;
            existingPatient.Email = email?.Trim() ?? existingPatient.Email;
            existingPatient.Phone = phone?.Trim() ?? existingPatient.Phone;
            if (!string.IsNullOrWhiteSpace(cityName) &&
                !string.IsNullOrWhiteSpace(streetName) &&
                houseNumber.HasValue && houseNumber.Value > 0 &&
                !string.IsNullOrWhiteSpace(postalCode))
            {
                if (cityName.Length < 2)
                    throw new InvalidAppointmentDataException("City name must be at least 2 characters");

                if (streetName.Length < 2)
                    throw new InvalidAppointmentDataException("Street name must be at least 2 characters");

                if (houseNumber.Value <= 0 || houseNumber.Value > 9999)
                    throw new InvalidAppointmentDataException("House number must be between 1 and 9999");

                if (postalCode.Length < 4 || postalCode.Length > 10)
                    throw new InvalidAppointmentDataException("Postal code must be between 4 and 10 characters");

                int newAddressId = await _addressManagement.CreateFullAddressAsync(
                    cityName, streetName, houseNumber.Value, postalCode);

                existingPatient.AddressId = newAddressId;
            }

            bool result = await _patientManagement.UpdatePatient(existingPatient);
            if (!result)
                throw new DatabaseException("Failed to update patient with address");

            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}