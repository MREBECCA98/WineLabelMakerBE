using System.ComponentModel.DataAnnotations;

//DTO used to send a quote email for the label
//The email will only be sent for the QuoteSent status

namespace WineLabelMakerBE.Models.DTOs.Email
{
    public class EmailQuoteDto
    {
        [Required]
        public Guid RequestId { get; set; }

        [Required]
        public string CustomBody { get; set; }
    }
}
