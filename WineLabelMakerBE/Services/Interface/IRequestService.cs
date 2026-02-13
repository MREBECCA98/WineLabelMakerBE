using WineLabelMakerBE.Models.DTOs.Requests;
using WineLabelMakerBE.Models.Entity;

//IRequestService defines the available methods for the Request Service
//Acts as a contract between Controller and Service, so the Controller knows
//which operations it can perform without needing to know internal logic

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
