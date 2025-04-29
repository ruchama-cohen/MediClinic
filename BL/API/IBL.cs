using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.API
{
    internal interface IBL
    {
        public IAuthService AuthService { get; set; }
        public IPatientService PatientService { get; set; }
    }
}
