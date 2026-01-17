using System.ComponentModel.DataAnnotations;

//I DTO (Data Transfer Object) vengono utilizzati per trasferire i dati tra client e server
//Vengono utilizzati per esporre solo i dati necessari per una determinata operazione,
//nascondendo quelli sensibili o non necessari. 
//In questo caso, il DTO "UpdateRequestDescriptionDto" rappresenta i dati necessari per aggiornare
//la descrizione esistente di una richiesta solo all'utente.
namespace WineLabelMakerBE.Models.DTOs.Requests
{
    public class UpdateRequestDescriptionDto
    {
        [Required]
        [MaxLength(5000)]
        public string Description { get; set; }
    }
}
