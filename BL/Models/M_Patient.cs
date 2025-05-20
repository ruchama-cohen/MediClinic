using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Models
{
    public class M_Patient
    {
        public string PatientName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string PatientPassword { get; set; } = null!;

        public int AddressId { get; set; }
    }
}
