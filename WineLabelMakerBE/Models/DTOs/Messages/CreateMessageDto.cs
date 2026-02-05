//using System.ComponentModel.DataAnnotations;

////I DTO (Data Transfer Object) vengono utilizzati per trasferire i dati tra client e server
////Vengono utilizzati per esporre solo i dati necessari per una determinata operazione,
////nascondendo quelli sensibili o non necessari. 
////In questo caso, il DTO "CreateMessageDto" viene utilizzato per inviare i dati necessari per creare
////una nuovi messaggi, esponendo solo il testo del messaggio e l'immagine dell'etichetta se presente.
//namespace WineLabelMakerBE.Models.DTOs.Messages
//{
//    public class CreateMessageDto
//    {
//        [Required]
//        [MaxLength(5000)]
//        public string Text { get; set; }

//        public string? ImageUrl { get; set; }

//    }
//}
