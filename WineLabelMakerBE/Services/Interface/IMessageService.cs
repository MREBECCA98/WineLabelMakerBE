//using WineLabelMakerBE.Models.DTOs.Messages;
//using WineLabelMakerBE.Models.Entity;

////L'interfaccia IMessageService definisce i metodi disponibili per il Service dei messaggi.
////Serve come "contratto" tra Controller e Service, così il Controller sa quali operazioni può eseguire
////senza conoscere i dettagli interni della logica, mentre il Service implementa l'interfaccia
//namespace WineLabelMakerBE.Services.Interface
//{
//    public interface IMessageService
//    {
//        //GET MESSAGES BY REQUEST ID
//        Task<List<Message>> GetMessagesByRequestIdAsync(Guid requestId, string userId, bool isAdmin);

//        //CREATE MESSAGE 
//        Task<Message> CreateMessageAsync(CreateMessageDto dto, string userId, Guid requestId, bool isAdmin);
//    }
//}
