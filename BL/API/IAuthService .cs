using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.API
{
     public interface IAuthService
    {
        Task<int> Login(int id, string password);
        Task<bool> SignIn(int id);


    }
}
