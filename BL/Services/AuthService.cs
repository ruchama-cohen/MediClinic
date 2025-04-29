using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.API;

namespace BLL.Services
{
    internal class AuthService : IAuthService
    {
        
        public Task<int> Login(int id, string password)
        {
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
