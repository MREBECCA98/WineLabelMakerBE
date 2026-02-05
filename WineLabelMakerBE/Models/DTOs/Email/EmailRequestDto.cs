using System.ComponentModel.DataAnnotations;

//DTO utilizzato per inviare un'email relativa a una richiesta completata.
//L'email verrà inviata solo se lo status della richiesta è "Completed".
//CustomBody permette di specificare un testo personalizzato se non viene specificato (EmailService)
namespace WineLabelMakerBE.Models.DTOs.Email
{
    public class EmailRequestDto
    {
        [Required]
        public Guid RequestId { get; set; }   //ID della richiesta

        public string? CustomBody { get; set; }
        [Required]
        public string ImageName { get; set; }  //solo per Completed
    }
}
