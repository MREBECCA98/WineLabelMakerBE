using System.ComponentModel.DataAnnotations;

//DTO used to send an email for a completed request
//The email is sent only when the request status is "Completed"
//CustomBody allows specifying custom text (if none is provided, a default from EmailService is used) 
//and a label image can be attached (required)

namespace WineLabelMakerBE.Models.DTOs.Email
{
    public class EmailRequestDto
    {
        [Required]
        public Guid RequestId { get; set; }

        public string? CustomBody { get; set; }
        [Required]
        public string ImageName { get; set; }
    }
}
