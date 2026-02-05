//using System.ComponentModel.DataAnnotations;
//using WineLabelMakerBE.Models.DTOs.Messages;

////I DTO (Data Transfer Object) vengono utilizzati per trasferire i dati tra client e server
////Vengono utilizzati per esporre solo i dati necessari per una determinata operazione,
////nascondendo quelli sensibili o non necessari. 
////In questo caso, il DTO "RequestWithMessagesDto" rappresenta una richiesta insieme ai suoi messaggi.
////Include le informazioni principali della richiesta (ID, descrizione, stato, data di creazione)
////e una lista di messaggi associati (GetMessageDto), permettendo al client di mostrare
////la conversazione completa relativa a quella richiesta.
//namespace WineLabelMakerBE.Models.DTOs.Requests
//{
//    public class RequestWithMessagesDto
//    {
//        public Guid IdRequest { get; set; }

//        [Required]
//        public string Description { get; set; }

//        [Required]
//        public string Status { get; set; }

//        [Required]
//        public DateTime CreatedAt { get; set; }

//        //Lista creata per contenere tutti i messaggi associati a una speifica richiesta
//        //Utilizza il DTO GetMessageDto per rappresentare ogni messaggio nella lista
//        [Required]
//        public List<GetMessageDto> Messages { get; set; }
//    }
//}
