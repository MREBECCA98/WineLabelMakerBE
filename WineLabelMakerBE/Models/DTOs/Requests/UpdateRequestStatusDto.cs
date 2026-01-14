using System.ComponentModel.DataAnnotations;
using WineLabelMakerBE.Models.Entity;

//I DTO (Data Transfer Object) vengono utilizzati per trasferire i dati tra client e server
//Vengono utilizzati per esporre solo i dati necessari per una determinata operazione,
//nascondendo quelli sensibili o non necessari. 
//In questo caso, il DTO "UpdateRequestStatusDto" rappresenta i dati necessari per aggiornare
//lo stato esistente.
//Questo aggiornamento dello stato può essere effettuato solo da un amministratore.
namespace WineLabelMakerBE.Models.DTOs.Requests
{
    public class UpdateRequestStatusDto
    {
        [Required]
        public RequestStatus Status { get; set; }
    }
}
