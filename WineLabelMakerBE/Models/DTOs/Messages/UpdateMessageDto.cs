using System.ComponentModel.DataAnnotations;

//I DTO (Data Transfer Object) vengono utilizzati per trasferire i dati tra client e server
//Vengono utilizzati per esporre solo i dati necessari per una determinata operazione,
//nascondendo quelli sensibili o non necessari. 
//In questo caso, il DTO "UpdateMessageDto" rappresenta i dati necessari per aggiornare un messaggio esistente,
//includendo il testo del messaggio e l'URL dell'immagine (se presente).
namespace WineLabelMakerBE.Models.DTOs.Messages
{
    public class UpdateMessageDto
    {
        [Required]
        [MaxLength(5000)]
        public string Text { get; set; }

        public string? ImageUrl { get; set; }
    }
}
