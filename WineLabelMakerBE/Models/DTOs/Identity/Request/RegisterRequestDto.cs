using System.ComponentModel.DataAnnotations;

namespace WineLabelMakerBE.Models.DTOs.Identity.Request
{
    public class RegisterRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }


        public DateTime Birthday { get; set; }
    }
}
