using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.API;
using BLL.Models;
using DAL.API;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientsManagement _patientManagement;
        private readonly IAddressManagement _addressManagement;
        private readonly AddressService _addressService;

        public PatientService(IPatientsManagement patientManagement, IAddressManagement addressManagement)
        {
            _patientManagement = patientManagement;
            _addressManagement = addressManagement;
            _addressService = new AddressService(patientManagement, addressManagement);
        }

        public async Task<bool> ChangePassword(string oldPassword, string newPassword)
        {
            // TODO: implement password change logic
            return false;
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
    }
}
