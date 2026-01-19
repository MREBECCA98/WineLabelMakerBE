using System.ComponentModel.DataAnnotations;

//I DTO (Data Transfer Object) vengono utilizzati per trasferire i dati tra client e server
//Vengono utilizzati per esporre solo i dati necessari per una determinata operazione,
//nascondendo quelli sensibili o non necessari
//In questo caso, il DTO "GetMessageDto" rappresenta i dati di un messaggio che vengono restituili al client,
//includendo informazioni come l'ID del messaggio, lo UserName e l'email dell'utente, il testo del messaggio,
//l'URL dell'immagine (se presente) e la data di creazione del messaggio
namespace WineLabelMakerBE.Models.DTOs.Messages
{
    public class GetMessageDto
    {
        public Guid IdMessage { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string UserEmail { get; set; }

        [Required]
        public string Text { get; set; }

        public string? ImageUrl { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
