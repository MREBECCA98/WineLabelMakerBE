using System.ComponentModel.DataAnnotations;

namespace WineLabelMakerBE.Models.DTOs.Identity.Request
{
    public class LoginRequestDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
