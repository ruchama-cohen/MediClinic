using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Models
{
    public class SignInRequest
    {
        [Required(ErrorMessage = "UserId is required.")]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "UserId must be 9 characters long.")]
        int UserId { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(15, MinimumLength = 4, ErrorMessage = "Password must be between 4 and 15 characters long.")]  
        string Password { get; set; }
    }
}
