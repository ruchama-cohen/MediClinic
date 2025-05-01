using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Models
{
    public class PatientVerificationRequest
    {
        [Required(ErrorMessage = "UserId is required.")]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "UserId must be 9 characters long.")]
        int UserId { get; set; }
        [Required(ErrorMessage = "User phone number is required.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "User phone number must be 10 characters long.")]
        string UserPhone { get; set; }
        [Required(ErrorMessage = "User birth date is required.")]
        [DataType(DataType.Date)]
        public DateOnly BirthDate { get; set; }

    }
}
