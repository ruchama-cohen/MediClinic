using System;
using System.ComponentModel.DataAnnotations;

namespace BLL.Models
{
    public class PatientVerificationRequest
    {
        [Required(ErrorMessage = "UserId is required.")]
        public int UserId { get; set; } 

        [Required(ErrorMessage = "User phone number is required.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "User phone number must be 10 characters long.")]
        public string UserPhone { get; set; } = string.Empty; 

        [Required(ErrorMessage = "User birth date is required.")]
        [DataType(DataType.Date)]
        public DateOnly BirthDate { get; set; }
    }
}
