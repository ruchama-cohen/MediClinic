using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class UpdatePatientRequest
    {
        [Required(ErrorMessage = "Patient Key is required")]
        public int PatientKey { get; set; }

        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string? PatientName { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }

        [StringLength(15, MinimumLength = 10, ErrorMessage = "Phone must be between 10 and 15 characters")]
        public string? Phone { get; set; }

        public AddressRequest? Address { get; set; }
    }

    public class AddressRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "Valid city must be selected")]
        public int? CityId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Valid street must be selected")]
        public int? StreetId { get; set; }

        [Range(1, 9999, ErrorMessage = "House number must be between 1 and 9999")]
        public int? HouseNumber { get; set; }

        [StringLength(10, MinimumLength = 4, ErrorMessage = "Postal code must be between 4 and 10 characters")]
        public string? PostalCode { get; set; }
    }

    public class PatientResponse
    {
        public int PatientKey { get; set; }
        public string PatientId { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public AddressResponse? Address { get; set; }
    }

    public class AddressResponse
    {
        public int CityId { get; set; }
        public string CityName { get; set; } = string.Empty;
        public int StreetId { get; set; }
        public string StreetName { get; set; } = string.Empty;
        public int HouseNumber { get; set; }
        public string PostalCode { get; set; } = string.Empty;
    }

    public class ChangePasswordRequest
    {
        [Required(ErrorMessage = "Patient Key is required")]
        public int PatientKey { get; set; }

        [Required(ErrorMessage = "Current password is required")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required")]
        [StringLength(15, MinimumLength = 4, ErrorMessage = "Password must be between 4 and 15 characters")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class CityResponse
    {
        public int CityId { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class StreetResponse
    {
        public int StreetId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CityId { get; set; }
    }
}