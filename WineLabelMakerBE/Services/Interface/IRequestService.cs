using WineLabelMakerBE.Models.DTOs.Requests;
using WineLabelMakerBE.Models.Entity;

//L'interfaccia IRequestService definisce i metodi disponibili per il Service delle richieste.
//Serve come "contratto" tra Controller e Service, così il Controller sa quali operazioni può eseguire
//senza conoscere i dettagli interni della logica, mentre il Service implementa l'interfaccia
namespace WineLabelMakerBE.Services.Interface
{
    public interface IRequestService
    {
        //GET ALL REQUESTS AS NO TRACKING
        Task<List<Request>> GetAllRequestsAsync();

        //GET REQUEST BY ID AS NO TRACKING
        Task<Request> GetRequestsByIdAsync(Guid id);

        //GET ALL REQUEST WITH MESSAGE
        //Task<List<RequestWithMessagesDto>> GetAllRequestsWithMessagesAsync(string userId, bool isAdmin);

        //POST CREATE REQUEST 
        Task<Request> CreateRequestAsync(CreateRequestDto dto, string userId);

        //SAVE FOR UPDATE
        Task<bool> Save();

        //GET BY ID FOR DELETE (NO AS NO TRACKING)
        Task<Request> GetRequestsById(Guid id);

        //DELETE 
        Task<bool> DeleteRequestAsync(Guid id);
    }
}
