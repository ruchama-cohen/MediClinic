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

namespace BLL.Services
{
    internal class AuthService : IAuthService
    {
        IPatientsManagement patientsManagement;
        public AuthService(IPatientsManagement _patientsManagement)
        {
            patientsManagement = _patientsManagement;
        }
        public async Task<int> Login(LogInRequest logInRequest)
        {
            Patient patient = await patientsManagement.GetPatientById(logInRequest.UserId);
            //exist:
            //function in patientmanagement which compare and check the values
            //return according to the function return
        }

        public Task<bool> SignIn(int id)
        {
            //exist:
            //function in patientmanagement which compare and check the values
            //return according to the function return
        }
    }
}
