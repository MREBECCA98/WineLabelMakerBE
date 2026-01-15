using WineLabelMakerBE.Models.Data;
using WineLabelMakerBE.Services.Interface;

namespace WineLabelMakerBE.Services
{
    public class MessageService : IMessageService
    {
        private readonly ApplicationDbContext _context;

        public MessageService(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}
