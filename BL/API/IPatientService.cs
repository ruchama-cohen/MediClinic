using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.API
{
    internal interface IPatientService
    {
        public async Task<bool> ChangePassword(string oldPassword, string newPassword);

    }
}
