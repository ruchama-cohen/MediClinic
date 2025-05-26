using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.API
{
    public interface IBL
    {
        public IAuthService AuthService { get; set; }
        public IPatientService PatientService { get; set; }
        public IAppointmentService AppointmentService { get; set; }
    }
}
