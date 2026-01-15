using WineLabelMakerBE.Models.DTOs.Requests;
using WineLabelMakerBE.Models.Entity;

namespace WineLabelMakerBE.Services.Interface
{
    public interface IRequestService
    {
        //GET ALL REQUESTS AS NO TRACKING
        Task<List<Request>> GetAllRequestsAsync();

        //GET REQUEST BY ID AS NO TRACKING
        Task<Request> GetRequestsByIdAsync(Guid id);

        //GET SEARCH AS NO TRACKING
        Task<List<Request>> GetRequestSearchAsync(string SearchTerm);

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
