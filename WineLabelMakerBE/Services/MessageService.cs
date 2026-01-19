using Microsoft.EntityFrameworkCore;
using WineLabelMakerBE.Models.Data;
using WineLabelMakerBE.Models.DTOs.Messages;
using WineLabelMakerBE.Models.Entity;
using WineLabelMakerBE.Services.Interface;

//Il Service gestisce tutta la logica delle richieste, CRUD, in questo caso solo Create e Read
//Eredita dall'interfaccia IMessageService, che specifica quali metodi sono disponibili
//per il controller, in modo tale che lavora con i dati senza occuparsi dei dettagli interni.
//Il Service lavora direttamente con le Entity e non con i DTO, che vengono invece usati dal Controller
//per comunicare con il client e non esporre dati sensibili.
namespace WineLabelMakerBE.Services
{
    public class MessageService : IMessageService
    {
        private readonly ApplicationDbContext _context;

        public MessageService(ApplicationDbContext context)
        {
            _context = context;
        }

        //GET MESSAGES BY REQUEST ID
        public async Task<List<Message>> GetMessagesByRequestIdAsync(Guid requestId, string userId, bool isAdmin)
        {
            var query = _context.Messages
                .Where(m => m.IdRequest == requestId)
                .Include(m => m.User)
                .AsQueryable();

            //Se l'utente non è admin può visualizzare solo i messaggi relativi alle sue richieste 
            if (!isAdmin)
            {
                query = query.Where(m => m.UserId == userId || m.Request.UserId == userId);
            }

            var messages = await query.OrderBy(m => m.CreatedAt).ToListAsync();

            return messages;
        }

        //CREATE MESSAGE 
        public async Task<Message> CreateMessageAsync(CreateMessageDto dto, string userId, Guid requestId, bool isAdmin)
        {
            var request = await _context.Requests
                .FirstOrDefaultAsync(r => r.IdRequest == requestId);

            if (request == null)
                return null;

            if (!isAdmin && request.UserId != userId)
                return null;

            var message = new Message
            {
                IdMessage = Guid.NewGuid(),
                Text = dto.Text,
                ImageUrl = dto.ImageUrl,
                CreatedAt = DateTime.UtcNow,
                IdRequest = requestId,
                UserId = userId
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return await _context.Messages
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.IdMessage == message.IdMessage);
        }

    }
}
