using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Models
{
    public class LogInRequest
    {
        [Required(ErrorMessage = "UserId is required.")]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "UserId must be 9 characters long.")]
        int UserId { get; set; }
    }
}
