using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.API;

namespace BLL.Services
{
    internal class PatientService:IPatientService
    {
        
        public async Task<bool> ChangePassword(string oldPassword, string newPassword)
        {
            //after the user insid'
            //function which return the old one compare it with the old password
            //if the old password is correct
            //change in the dall-send object model whith the new password
            //return true
            //else return false
        }

    }
}
