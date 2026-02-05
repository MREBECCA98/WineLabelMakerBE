using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WineLabelMakerBE.Models.DTOs.Requests;
using WineLabelMakerBE.Models.Entity;
using WineLabelMakerBE.Services.Interface;

//Il Controller fa da filtro tra le richieste del client e le risposte del server.
//Riceve le richieste dal client, passa i dati al service che gestisce la logica,
//e infine restituisce al client solo le informazioni necessarie tramite i DTO,
//per non esporre dati sensibili.
//In questo caso, si tratta di un Controller per le richieste/descrizioni del prodotto
//che il client invia per mettersi in contatto con l'azienda.
namespace WineLabelMakerBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        //Iniezione dell'interfaccia per la gestione delle richieste 
        private readonly IRequestService _requestService;
        private readonly IEmailService _emailService;

        public RequestController(IRequestService requestService, IEmailService emailService)
        {
            _emailService = emailService;
            _requestService = requestService;
        }

        //GET ALL REQUESTS AS NO TRACKING
        //Questo endpoint restituisce tutte le richieste filtrate in base al ruolo dell'utente:
        //-L'admin vede tutte le richieste di tutti gli utenti
        //-L'user vede solo le proprie richieste
        [HttpGet("allRequest")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<GetRequestDto>>> GetAllRequests()
        {
            try
            {
                string userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                bool isAdmin = User.IsInRole("Admin");

                var requests = (await _requestService.GetAllRequestsAsync())
                    .Where(r => isAdmin || r.UserId == userId);

                var requestDto = requests.Select(r => new GetRequestDto
                {
                    IdRequest = r.IdRequest,
                    Description = r.Description,
                    Status = r.Status.ToString(),
                    CreatedAt = r.CreatedAt,
                    UserName = r.User.Name,
                    UserSurname = r.User.Surname,
                    UserEmail = r.User.Email,
                    CompanyName = r.User.CompanyName,
                }).ToList();

                return Ok(requestDto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Errore interno: {ex.Message}");
            }
        }

        //GET REQUEST BY ID AS NO TRACKING
        //Questo endpoint restituisce una singola richiesta filtrata in base al ruolo dell'utente:
        //-L'admin può vedere qualsiasi richiesta
        //-L'user può vedere solo le proprie richieste
        [HttpGet("requestById/{id:guid}")]
        [Authorize]
        public async Task<ActionResult<GetRequestDto>> GetRequestById(Guid id)
        {
            try
            {
                if (Guid.Empty == id)
                    return BadRequest("Id non valido");

                var request = await _requestService.GetRequestsByIdAsync(id);

                if (request == null)
                    return NotFound();

                string userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                bool isAdmin = User.IsInRole("Admin");

                if (!isAdmin && request.UserId != userId)
                    return Forbid();

                var requestDto = new GetRequestDto
                {
                    IdRequest = request.IdRequest,
                    Description = request.Description,
                    Status = request.Status.ToString(),
                    CreatedAt = request.CreatedAt,
                    UserName = request.User.Name,
                    UserSurname = request.User.Surname,
                    UserEmail = request.User.Email,
                    CompanyName = request.User.CompanyName,
                };

                return Ok(requestDto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Errore interno: {ex.Message}");
            }
        }

        //GET ALL REQUEST WITH MESSAGE
        //[HttpGet("allWithMessages")]
        //[Authorize]
        //public async Task<IActionResult> GetAllRequestsWithMessages()
        //{
        //    string userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        //    bool isAdmin = User.IsInRole("Admin");

        //    var result = await _requestService.GetAllRequestsWithMessagesAsync(userId, isAdmin);

        //    return Ok(result);
        //}

        //POST CREATE REQUEST
        //Questo endpoint è accessibile solo all'user
        //Permette all'utente di creare una nuova richiesta al server
        [HttpPost("createRequest")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreateRequest(CreateRequestDto requestDto)
        {
            try
            {
                string userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("Utente non autenticato");
                }

                var createdRequest = await _requestService.CreateRequestAsync(requestDto, userId);

                var resultDto = new GetRequestDto
                {
                    IdRequest = createdRequest.IdRequest,
                    Description = createdRequest.Description,
                    Status = createdRequest.Status.ToString(),
                    CreatedAt = createdRequest.CreatedAt,
                    UserName = createdRequest.User.Name,
                    UserSurname = createdRequest.User.Surname,
                    UserEmail = createdRequest.User.Email,
                    CompanyName = createdRequest.User.CompanyName,
                };

                return Ok(resultDto);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError, $"Errore interno: {ex.Message}");
            }
        }

        //UPDATE CLIENT DESCRIPTION
        //Questo endpoint è accessibile solo all'user
        //Permette all'utente di modificare la richiesta già esistente 
        [HttpPut("updateClient/{id:guid}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> UpdateRequestDescription(Guid id, UpdateRequestDescriptionDto descriptionDto)
        {
            try
            {
                if (descriptionDto == null || string.IsNullOrWhiteSpace(descriptionDto.Description))
                    return BadRequest("Descrizione non valida");

                var request = await _requestService.GetRequestsById(id);
                if (request == null)
                    return NotFound("Richiesta non trovata");

                request.Description = descriptionDto.Description;
                request.UpdatedAt = DateTime.UtcNow;

                bool saved = await _requestService.Save();

                if (!saved)
                    return BadRequest("Impossibile aggiornare la richiesta");

                var resultDto = new GetRequestDto
                {
                    IdRequest = request.IdRequest,
                    Description = request.Description,
                    Status = request.Status.ToString(),
                    CreatedAt = request.CreatedAt,
                    UserName = request.User.Name,
                    UserSurname = request.User.Surname,
                    UserEmail = request.User.Email,
                    CompanyName = request.User.CompanyName,
                };

                return Ok(resultDto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Errore interno: {ex.Message}");
            }
        }


        //UPDATE ADMIN STATUS
        //Questo endpoint è accessibile solo all'admin
        //Permette all'admin di modificare lo stato della richiesta (in attesa, in corso, completata, rifiutata)
        //E invia un'email di default in base allo stato cambiato
        [HttpPut("updateAdmin/{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRequestStatus(Guid id, UpdateRequestStatusDto statusDto)
        {
            try
            {
                if (statusDto == null)
                    return BadRequest("Dati non validi");

                var request = await _requestService.GetRequestsById(id);
                if (request == null)
                    return NotFound("Richiesta non trovata");

                request.Status = statusDto.Status;
                request.UpdatedAt = DateTime.UtcNow;

                bool saved = await _requestService.Save();
                if (!saved)
                    return BadRequest("Impossibile aggiornare lo stato");


                //EMAIL DI DEFAULT IN BASE AL CAMBIO DI STATO 
                //--------------------------------------------------------------------------------------
                string statusIT = request.Status switch
                {
                    RequestStatus.Pending => "In attesa",
                    RequestStatus.InProgress => "In lavorazione",
                    RequestStatus.Completed => "Completata",
                    RequestStatus.Rejected => "Respinta",


                };
                string subject = $"WINE LABEL MAKER - aggiornamento richiesta: {statusIT}";
                string body = request.Status switch
                {


                    RequestStatus.InProgress => $"Gentile {request.User.Name} {request.User.Surname},\n\n" +
                                                "Abbiamo preso in carico la sua richiesta per la nuova etichetta di vino. " +
                                                "La nostra illustratrice sta attualmente lavorando sulla creazione dell’etichetta, " +
                                                "seguendo le indicazioni che ci ha fornito.\n\n" +
                                                "Non appena l’etichetta sarà completata, riceverà una nuova email con il file pronto.\n\n" +
                                                "Grazie per aver scelto Wine Label Maker.",
                    RequestStatus.Rejected => $"Gentile {request.User.Name} {request.User.Surname},\n\n" +
                                              "Siamo spiacenti di informarla che la sua richiesta per la nuova etichetta di vino non può essere completata. " +
                                              "Se desidera ulteriori dettagli o assistenza, non esiti a contattarci.\n\n" +
                                              "Ci auguriamo di poterla aiutare con altre richieste in futuro.\n\n" +
                                              "Grazie per aver scelto Wine Label Maker.",

                    _ => ""


                };

                //Se il body e null non inviare la mail
                if (!string.IsNullOrEmpty(body))
                {
                    await _emailService.SendSimpleEmailAsync(request.User.Email, subject, body);
                }

                //--------------------------------------------------------------------------------------

                var resultDto = new GetRequestDto
                {
                    IdRequest = request.IdRequest,
                    Description = request.Description,
                    Status = request.Status.ToString(),
                    CreatedAt = request.CreatedAt,
                    UserName = request.User.Name,
                    UserSurname = request.User.Surname,
                    UserEmail = request.User.Email,
                    CompanyName = request.User.CompanyName,
                };

                return Ok(resultDto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Errore interno: {ex.Message}");
            }
        }

        //DELETE REQUEST
        //Questo endpoint permette di eliminare le richieste già esistenti in base al ruolo dell'utente:
        //-L'admin può cancellare le richieste di tutti gli utenti
        //-L'user può cancellare solo le proprie richieste
        [HttpDelete("deleteRequest/{id:guid}")]
        [Authorize]
        public async Task<IActionResult> DeleteRequest(Guid id)
        {
            try
            {
                if (Guid.Empty == id)
                    return BadRequest("Id non valido");

                var request = await _requestService.GetRequestsById(id);
                if (request == null)
                    return NotFound("Richiesta non trovata");

                string userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                bool isAdmin = User.IsInRole("Admin");

                if (!isAdmin && request.UserId != userId)
                    return Forbid("Non puoi eliminare questa richiesta");

                bool result = await this._requestService.DeleteRequestAsync(id);
                return result ? Ok("Richiesta eliminata con successo") : BadRequest("Impossibile eliminare la richiesta");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Errore interno: {ex.Message}");
            }
        }

    }
}
