using Microsoft.EntityFrameworkCore;
using WineLabelMakerBE.Models.Data;
//using WineLabelMakerBE.Models.DTOs.Messages;
using WineLabelMakerBE.Models.DTOs.Requests;
using WineLabelMakerBE.Models.Entity;
using WineLabelMakerBE.Services.Interface;

//The Service handles all the logic for requests, including CRUD (create, read, update, delete)
//It implements the IRequestService interface, which specifies which methods are available
//for the controller, so that it works with the data without worrying about internal details.
//The Service works directly with Entities and not DTOs, which are used by the Controller
//to communicate with the client without exposing sensitive data.

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

        //GET ALL REQUEST WITH MESSAGE
        //public async Task<List<RequestWithMessagesDto>> GetAllRequestsWithMessagesAsync(string userId, bool isAdmin)
        //{
        //    var query = _context.Requests
        //        .Include(r => r.Messages)
        //        .ThenInclude(m => m.User)
        //        .AsQueryable();

        //    if (!isAdmin)
        //    {
        //        query = query.Where(r => r.UserId == userId);
        //    }

        //    var requests = await query
        //        .OrderByDescending(r => r.CreatedAt)
        //        .ToListAsync();

        //    return requests.Select(r => new RequestWithMessagesDto
        //    {
        //        IdRequest = r.IdRequest,
        //        Description = r.Description,
        //        Status = r.Status.ToString(),
        //        CreatedAt = r.CreatedAt,
        //        Messages = r.Messages
        //            .OrderBy(m => m.CreatedAt)
        //            .Select(m => new GetMessageDto
        //            {
        //                IdMessage = m.IdMessage,
        //                Text = m.Text,
        //                ImageUrl = m.ImageUrl,
        //                CreatedAt = m.CreatedAt,
        //                UserName = m.User.UserName,
        //                UserEmail = m.User.Email
        //            }).ToList(),

        //    }).ToList();
        //}

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
