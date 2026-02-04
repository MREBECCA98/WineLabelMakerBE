using System.ComponentModel.DataAnnotations;

//I DTO (Data Transfer Object) vengono utilizzati per trasferire i dati tra client e server
//Vengono utilizzati per esporre solo i dati necessari per una determinata operazione,
//nascondendo quelli sensibili o non necessari. 
//In questo caso, il DTO "GetRequestDto" rappresenta i dati di una richiesta
//che verranno restituiti al client dal server,
//includendo informazioni come l'ID della richiesta, il nome e l'email dell'utente,
//la descrizione della richiesta, lo stato e la data di creazione.
namespace WineLabelMakerBE.Models.DTOs.Requests
{
    public class GetRequestDto
    {
        public Guid IdRequest { get; set; }

        [Required]
        [MaxLength(100)]
        public string CompanyName { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string UserSurname { get; set; }

        [Required]
        [EmailAddress]
        public string UserEmail { get; set; }

        [Required]
        [MaxLength(5000)]
        public string Description { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
