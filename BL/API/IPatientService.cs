using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.API
{
    public interface IPatientService
    {
        Task<bool> ChangePassword(string oldPassword, string newPassword);

    }
}
