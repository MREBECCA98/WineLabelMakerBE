using System.ComponentModel.DataAnnotations;

namespace WineLabelMakerBE.Models.DTOs.Identity.Request
{
    public class RegisterRequestDto
    {
        [Required]
        [MaxLength(100)]
        public string CompanyName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

        public string? Name { get; set; }

        public string? Surname { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
    }
}
