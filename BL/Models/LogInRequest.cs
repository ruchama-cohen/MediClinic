using System.ComponentModel.DataAnnotations;

namespace BLL.Models
{
    public class LogInRequest
    {
        [Required(ErrorMessage = "User ID is required.")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "User password is required.")]
        [StringLength(15, MinimumLength = 4, ErrorMessage = "Password must be between 4 and 15 characters long.")]
        public string UserPassword { get; set; } = string.Empty;
    }
}