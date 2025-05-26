using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;
namespace BLL.Models
{
    public class PatientModel
    {
        public int PatientKey { get; set; }
        public string PatientId { get; set; } = null!;
        public string PatientName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string PatientPassword { get; set; } = null!;

        public  Address address{ get; set; }
    }
}
