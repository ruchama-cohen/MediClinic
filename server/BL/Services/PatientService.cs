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
            Console.WriteLine($"=== UpdatePatientPartial START ===");
            Console.WriteLine($"PatientKey: {patientKey}");
            Console.WriteLine($"Name: '{name}'");
            Console.WriteLine($"Email: '{email}'");
            Console.WriteLine($"Phone: '{phone}'");
            Console.WriteLine($"CityId: {cityId}");
            Console.WriteLine($"StreetId: {streetId}");
            Console.WriteLine($"HouseNumber: {houseNumber}");
            Console.WriteLine($"PostalCode: '{postalCode}'");

            if (patientKey <= 0)
                throw new InvalidAppointmentDataException("Patient key is required");

            var existingPatient = await _patientManagement.GetPatientByIdString(patientKey);
            if (existingPatient == null)
                throw new PatientNotFoundException(patientKey.ToString());

            Console.WriteLine($"Found existing patient: {existingPatient.PatientName}");
            bool hasChanges = false;

            // עדכון שם אם סופק
            if (!string.IsNullOrWhiteSpace(name))
            {
                string trimmedName = name.Trim();
                if (trimmedName.Length < 2)
                    throw new InvalidAppointmentDataException("Name must be at least 2 characters");

                if (trimmedName != existingPatient.PatientName)
                {
                    Console.WriteLine($"Updating name from '{existingPatient.PatientName}' to '{trimmedName}'");
                    existingPatient.PatientName = trimmedName;
                    hasChanges = true;
                }
            }

            // עדכון אימייל אם סופק
            if (!string.IsNullOrWhiteSpace(email))
            {
                string trimmedEmail = email.Trim();
                if (!IsValidEmail(trimmedEmail))
                    throw new InvalidAppointmentDataException("Valid email is required");

                if (trimmedEmail != existingPatient.Email)
                {
                    Console.WriteLine($"Updating email from '{existingPatient.Email}' to '{trimmedEmail}'");
                    existingPatient.Email = trimmedEmail;
                    hasChanges = true;
                }
            }

            // עדכון טלפון אם סופק
            if (!string.IsNullOrWhiteSpace(phone))
            {
                string trimmedPhone = phone.Trim();
                if (trimmedPhone.Length < 10 || trimmedPhone.Length > 15)
                    throw new InvalidAppointmentDataException("Phone must be between 10 and 15 characters");

                if (trimmedPhone != existingPatient.Phone)
                {
                    Console.WriteLine($"Updating phone from '{existingPatient.Phone}' to '{trimmedPhone}'");
                    existingPatient.Phone = trimmedPhone;
                    hasChanges = true;
                }
            }

            // עדכון כתובת - רק אם כל הפרטים סופקו או שכולם null
            bool hasAnyAddressData = cityId.HasValue || streetId.HasValue || houseNumber.HasValue || !string.IsNullOrWhiteSpace(postalCode);
            bool hasAllAddressData = cityId.HasValue && streetId.HasValue && houseNumber.HasValue && !string.IsNullOrWhiteSpace(postalCode);

            if (hasAnyAddressData)
            {
                if (!hasAllAddressData)
                {
                    throw new InvalidAppointmentDataException("To update address, all address fields must be provided (city, street, house number, postal code)");
                }

                // וולידציה של נתוני כתובת
                if (cityId.Value <= 0)
                    throw new InvalidAppointmentDataException("Valid city is required");

                if (streetId.Value <= 0)
                    throw new InvalidAppointmentDataException("Valid street is required");

                if (houseNumber.Value <= 0 || houseNumber.Value > 9999)
                    throw new InvalidAppointmentDataException("House number must be between 1 and 9999");

                string trimmedPostalCode = postalCode!.Trim();
                if (trimmedPostalCode.Length < 4 || trimmedPostalCode.Length > 10)
                    throw new InvalidAppointmentDataException("Postal code must be between 4 and 10 characters");

                Console.WriteLine($"Processing address update: City={cityId}, Street={streetId}, House={houseNumber}, Postal={trimmedPostalCode}");

                // בדיקה אם יש כתובת קיימת עם הפרטים האלה
                var existingAddress = await _addressManagement.FindExistingAddressAsync(
                    cityId.Value, streetId.Value, houseNumber.Value, trimmedPostalCode);

                int addressId;
                if (existingAddress != null)
                {
                    Console.WriteLine($"Found existing address: {existingAddress.AddressId}");
                    addressId = existingAddress.AddressId;
                }
                else
                {
                    Console.WriteLine("Creating new address");
                    // יצירת כתובת חדשה
                    var newAddress = new Address
                    {
                        CityId = cityId.Value,
                        StreetId = streetId.Value,
                        HouseNumber = houseNumber.Value,
                        PostalCode = trimmedPostalCode
                    };
                    addressId = await _addressManagement.AddAddress(newAddress);
                    Console.WriteLine($"Created new address with ID: {addressId}");
                }

                if (existingPatient.AddressId != addressId)
                {
                    Console.WriteLine($"Updating address ID from {existingPatient.AddressId} to {addressId}");
                    existingPatient.AddressId = addressId;
                    hasChanges = true;
                }
            }

            Console.WriteLine($"Has changes: {hasChanges}");

            if (!hasChanges)
            {
                Console.WriteLine("No changes detected, returning true");
                return true; // אין שינויים, אבל זה בסדר
            }

            Console.WriteLine("Saving changes to database");
            bool result = await _patientManagement.UpdatePatient(existingPatient);
            if (!result)
                throw new DatabaseException("Failed to update patient");

            Console.WriteLine($"=== UpdatePatientPartial END - Success: {result} ===");
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