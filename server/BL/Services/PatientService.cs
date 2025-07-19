using System;
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

        public async Task<bool> ChangePassword(int patientKey, string oldPassword, string newPassword)
        {
            if (patientKey <= 0)
                throw new InvalidAppointmentDataException("Patient key is required");

            if (string.IsNullOrWhiteSpace(oldPassword))
                throw new InvalidAppointmentDataException("Current password is required");

            if (string.IsNullOrWhiteSpace(newPassword))
                throw new InvalidAppointmentDataException("New password is required");

            if (newPassword.Length < 4 || newPassword.Length > 15)
                throw new InvalidAppointmentDataException("Password must be between 4 and 15 characters");

            var patient = await _patientManagement.GetPatientByIdString(patientKey);
            if (patient == null)
                throw new PatientNotFoundException(patientKey.ToString());

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

        public async Task<PatientModel?> GetPatientByKey(int patientKey)
        {
            if (patientKey <= 0)
                throw new InvalidAppointmentDataException("Patient key is required");

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

        public async Task<bool> UpdatePatientPartial(int patientKey, string? name = null, string? email = null, string? phone = null,
            int? cityId = null, int? streetId = null, int? houseNumber = null, string? postalCode = null)
        {
            if (patientKey <= 0)
                throw new InvalidAppointmentDataException("Patient key is required");

            var existingPatient = await _patientManagement.GetPatientByIdString(patientKey);
            if (existingPatient == null)
                throw new PatientNotFoundException(patientKey.ToString());

            bool hasChanges = false;

            // עדכון שם
            if (!string.IsNullOrWhiteSpace(name) && name != existingPatient.PatientName)
            {
                if (name.Trim().Length < 2)
                    throw new InvalidAppointmentDataException("Name must be at least 2 characters");
                existingPatient.PatientName = name.Trim();
                hasChanges = true;
            }

            // עדכון אימייל
            if (!string.IsNullOrWhiteSpace(email) && email != existingPatient.Email)
            {
                if (!IsValidEmail(email))
                    throw new InvalidAppointmentDataException("Valid email is required");
                existingPatient.Email = email.Trim();
                hasChanges = true;
            }

            // עדכון טלפון
            if (!string.IsNullOrWhiteSpace(phone) && phone != existingPatient.Phone)
            {
                if (phone.Length < 10 || phone.Length > 15)
                    throw new InvalidAppointmentDataException("Phone must be between 10 and 15 characters");
                existingPatient.Phone = phone.Trim();
                hasChanges = true;
            }

            // עדכון כתובת - רק אם כל הפרמטרים של הכתובת מועברים
            if (cityId.HasValue && streetId.HasValue && houseNumber.HasValue && !string.IsNullOrWhiteSpace(postalCode))
            {
                if (cityId.Value <= 0)
                    throw new InvalidAppointmentDataException("Valid city is required");

                if (streetId.Value <= 0)
                    throw new InvalidAppointmentDataException("Valid street is required");

                if (houseNumber.Value <= 0 || houseNumber.Value > 9999)
                    throw new InvalidAppointmentDataException("House number must be between 1 and 9999");

                if (postalCode.Length < 4 || postalCode.Length > 10)
                    throw new InvalidAppointmentDataException("Postal code must be between 4 and 10 characters");

                // בדיקה אם יש כתובת קיימת עם הפרטים האלה
                var existingAddress = await _addressManagement.FindExistingAddressAsync(
                    cityId.Value, streetId.Value, houseNumber.Value, postalCode.Trim());

                int addressId;
                if (existingAddress != null)
                {
                    addressId = existingAddress.AddressId;
                }
                else
                {
                    // יצירת כתובת חדשה
                    var newAddress = new Address
                    {
                        CityId = cityId.Value,
                        StreetId = streetId.Value,
                        HouseNumber = houseNumber.Value,
                        PostalCode = postalCode.Trim()
                    };
                    addressId = await _addressManagement.AddAddress(newAddress);
                }

                if (existingPatient.AddressId != addressId)
                {
                    existingPatient.AddressId = addressId;
                    hasChanges = true;
                }
            }

            if (!hasChanges)
                return true; // אין שינויים, אבל זה בסדר

            bool result = await _patientManagement.UpdatePatient(existingPatient);
            if (!result)
                throw new DatabaseException("Failed to update patient");

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
