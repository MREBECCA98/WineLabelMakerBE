using System.ComponentModel.DataAnnotations;
//I DTO (Data Transfer Object) vengono utilizzati per trasferire i dati tra client e server
//Vengono utilizzati per esporre solo i dati necessari per una determinata operazione,
//nascondendo quelli sensibili o non necessari. 
//In questo caso, il DTO "CreateRequestDto" viene utilizzato per inviare i dati necessari per creare una
//nuova richiesta, esponendo solo la descrizione.
namespace WineLabelMakerBE.Models.DTOs.Requests
{
    public class CreateRequestDto
    {
        [Required]
        [MaxLength(5000)]
        public string Description { get; set; }
    }
}
