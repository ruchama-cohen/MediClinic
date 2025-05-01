using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Models
{
    internal class SiteRegistrationRequest
    {
        [Required(ErrorMessage = "User password is required.")]
        [StringLength(15, MinimumLength = 4, ErrorMessage = "Password must be between 4 and 15 characters long.")]
        public string UserPassword { get; set; }
        [Required(ErrorMessage = "Verify פassword is required.")]
        [StringLength(15, MinimumLength = 4, ErrorMessage = "Password must be between 4 and 15 characters long.")]
        public string VerifyPassword { get; set; }
    }
}
