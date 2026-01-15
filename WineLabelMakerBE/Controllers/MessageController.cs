using Microsoft.AspNetCore.Mvc;
using WineLabelMakerBE.Services.Interface;

namespace WineLabelMakerBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

    }
}
