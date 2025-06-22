using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class UpdatePatientRequest
    {
        [Required(ErrorMessage = "Patient ID is required")]
        public string PatientId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string PatientName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone is required")]
        [StringLength(15, MinimumLength = 10, ErrorMessage = "Phone must be between 10 and 15 characters")]
        public string Phone { get; set; } = string.Empty;

        public AddressRequest? Address { get; set; }
    }

    public class AddressRequest
    {
        [Required(ErrorMessage = "City name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "City name must be between 2 and 100 characters")]
        public string CityName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Street name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Street name must be between 2 and 100 characters")]
        public string StreetName { get; set; } = string.Empty;

        [Required(ErrorMessage = "House number is required")]
        [Range(1, 9999, ErrorMessage = "House number must be between 1 and 9999")]
        public int HouseNumber { get; set; }

        [Required(ErrorMessage = "Postal code is required")]
        [StringLength(10, MinimumLength = 4, ErrorMessage = "Postal code must be between 4 and 10 characters")]
        public string PostalCode { get; set; } = string.Empty;
    }

    public class PatientResponse
    {
        public string PatientId { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public AddressResponse? Address { get; set; }
    }

    public class AddressResponse
    {
        public string CityName { get; set; } = string.Empty;
        public string StreetName { get; set; } = string.Empty;
        public int HouseNumber { get; set; }
        public string PostalCode { get; set; } = string.Empty;
    }

    public class ChangePasswordRequest
    {
        [Required(ErrorMessage = "Patient ID is required")]
        public string PatientId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Current password is required")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required")]
        [StringLength(15, MinimumLength = 4, ErrorMessage = "Password must be between 4 and 15 characters")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class UpdateContactInfoRequest
    {
        [Required(ErrorMessage = "Patient ID is required")]
        public string PatientId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2)]
        public string PatientName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone is required")]
        [StringLength(15, MinimumLength = 10)]
        public string Phone { get; set; } = string.Empty;
    }
}