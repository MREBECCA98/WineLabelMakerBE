using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WineLabelMakerBE.Models.DTOs.Messages;
using WineLabelMakerBE.Services.Interface;

//Il Controller fa da filtro tra le richieste del client e le risposte del server.
//Riceve le richieste dal client, passa i dati al service che gestisce la logica,
//e infine restituisce al client solo le informazioni necessarie tramite i DTO,
//per non esporre dati sensibili.
//In questo caso, si tratta di un Controller per i messaggi tra admin e client
//associati alle richieste di etichette di vino.
namespace WineLabelMakerBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        //Iniezione dell'interfaccia per la gestione delle richieste 
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        //GET MESSAGES BY REQUEST ID
        //Questo endpoint recuper tutti i messaggi associati a una specifica richiesta
        //-L'admin può visualizzare tutti i messaggi di una richiesta
        //-L'utente può visualizzare solo i messaggi associati alla propria richiesta
        [HttpGet("{requestId:guid}")]
        [Authorize]
        public async Task<IActionResult> GetMessages(Guid requestId)
        {
            try
            {
                string userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                bool isAdmin = User.IsInRole("Admin");

                var messages = await _messageService.GetMessagesByRequestIdAsync(requestId, userId, isAdmin);
                return Ok(messages.Select(m => new GetMessageDto
                {
                    IdMessage = m.IdMessage,
                    Text = m.Text,
                    ImageUrl = m.ImageUrl,
                    CreatedAt = m.CreatedAt,
                    UserName = m.User.Name,
                    UserSurname = m.User.Surname,
                    UserEmail = m.User.Email,
                    CompanyName = m.User.CompanyName
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Errore interno del server: " + ex.Message);
            }
        }

        //CREATE MESSAGE 
        //Questo endpoint consente di creare un nuovo messaggio associato a una richiesta specifica
        [HttpPost("{requestId:guid}")]
        [Authorize]
        public async Task<IActionResult> CreateMessage(Guid requestId, CreateMessageDto dto)
        {
            try
            {
                string userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                bool isAdmin = User.IsInRole("Admin");

                var message = await _messageService.CreateMessageAsync(dto, userId, requestId, isAdmin);

                if (message == null)
                    return BadRequest("Impossibile creare il messaggio");

                var messageDto = new GetMessageDto
                {
                    IdMessage = message.IdMessage,
                    Text = message.Text,
                    ImageUrl = message.ImageUrl,
                    CreatedAt = message.CreatedAt,
                    UserName = message.User.Name,
                    UserSurname = message.User.Surname,
                    UserEmail = message.User.Email,
                    CompanyName = message.User.CompanyName
                };

                return Ok(messageDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Errore interno del server: " + ex.Message);

            }

        }
    }
}
