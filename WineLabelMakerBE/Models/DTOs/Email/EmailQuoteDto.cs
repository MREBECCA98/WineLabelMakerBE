using System.ComponentModel.DataAnnotations;

//DTO utilizzato per inviare un'email relativa al preventivo per l'etichetta.
//L'email verrà inviata solo per lo status QuoteSent.
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
