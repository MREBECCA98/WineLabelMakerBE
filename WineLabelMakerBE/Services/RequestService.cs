using Microsoft.EntityFrameworkCore;
using WineLabelMakerBE.Models.Data;
using WineLabelMakerBE.Models.DTOs.Requests;
using WineLabelMakerBE.Models.Entity;
using WineLabelMakerBE.Services.Interface;

namespace WineLabelMakerBE.Services
{
    public class RequestService : IRequestService
    {
        private readonly ApplicationDbContext _context;

        public RequestService(ApplicationDbContext context)
        {
            _context = context;
        }

        //GET ALL REQUESTS AS NO TRACKING
        public async Task<List<Request>> GetAllRequestsAsync()
        {
            return await this._context.Requests
                .Include(r => r.User)
                .AsNoTracking()
                .ToListAsync();
        }

        //GET REQUEST BY ID AS NO TRACKING
        public async Task<Request> GetRequestsByIdAsync(Guid id)
        {
            return await this._context.Requests
                .Include(r => r.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.IdRequest == id);
        }

        //GET SEARCH AS NO TRACKING
        public async Task<List<Request>> GetRequestSearchAsync(string SearchTerm)
        {
            if (string.IsNullOrWhiteSpace(SearchTerm))
                return new List<Request>();

            return await this._context.Requests
                .Include(r => r.User)
               .Where(s => EF.Functions.Like(s.User.UserName, $"%{SearchTerm}%"))
               .AsNoTracking()
               .ToListAsync();
        }

        //POST CREATE REQUEST 
        public async Task<Request> CreateRequestAsync(CreateRequestDto dto, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("UserId non valido.");

            var request = new Request
            {
                IdRequest = Guid.NewGuid(),
                Description = dto.Description,
                Status = RequestStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UserId = userId
            };

            await _context.Requests.AddAsync(request);
            await _context.SaveChangesAsync();

            var savedRequest = await _context.Requests
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.IdRequest == request.IdRequest);

            return savedRequest;
        }

        //SAVE FOR UPDATE
        public async Task<bool> Save()
        {
            return await this._context.SaveChangesAsync() > 0;
        }

        //GET BY ID FOR DELETE (NO AS NO TRACKING)
        public async Task<Request> GetRequestsById(Guid id)
        {
            return await this._context.Requests
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.IdRequest == id);
        }

        //DELETE 
        public async Task<bool> DeleteRequestAsync(Guid id)
        {
            Request request = await this.GetRequestsById(id);
            if (request is not null)
            {
                this._context.Remove(request);
                return await this._context.SaveChangesAsync() > 0;
            }


            return false;

        }
    }


}
