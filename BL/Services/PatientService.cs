using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.API;
using BLL.Models;
using DAL.API;
using DAL.Models;
using DAL.Services;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class PatientService:IPatientService
    {

        private readonly PatientsManagement _patientManagement;

        public async Task<bool> ChangePassword(string oldPassword, string newPassword)
        {
            //after the user insid'
            //function which return the old one compare it with the old password
            //if the old password is correct
            //change in the dall-send object model whith the new password
            //return true
            return false;
        }
        

            public PatientService(PatientsManagement patientManagement)
            {
            _patientManagement = patientManagement;
        }

        public async Task<bool> UpdatePatient(PatientModel model)
        {
            var existingPatient = await _patientManagement.GetPatientById(model.PatientKey);
            if (existingPatient == null)
                return false;
            var addressId = await AddressService.GetOrAddAddressAsync(model.address);

        var updatedEntity = new Patient
            {

            PatientKey = model.PatientKey,
            PatientId = model.PatientId,
            PatientName = model.PatientName,        
                Email = model.Email,            
                Phone = model.Phone,
                PatientPassword= model.PatientPassword,
            AddressId = model.address.AddressId,    
            Address=model.address,

        };

            return await _patientManagement.UpdatePatient(updatedEntity);
        }
       
        

           
        }




    }
