using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WineLabelMakerBE.Models.DTOs.Requests;
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

        public RequestController(IRequestService requestService)
        {
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
                    UserEmail = r.User.Email
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
                    UserName = request.User.UserName,
                    UserEmail = request.User.Email
                };

                return Ok(requestDto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Errore interno: {ex.Message}");
            }
        }

        //GET SEARCH 
        //Questo endpoint è accessibile solo dall'admin.
        //Permette di cercare le richieste filtrate per UserName
        //e restituisce ogni richiesta trovata con i relativi messaggi
        [HttpGet("searchRequest/{searchTerm}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<RequestWithMessagesDto>>> GetSearchRequest(string searchTerm)
        {
            try
            {
                var requests = await _requestService.GetRequestSearchAsync(searchTerm);

                return Ok(requests);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Errore interno: {ex.Message}");
            }
        }

        //GET ALL REQUEST WITH MESSAGE
        [HttpGet("allWithMessages")]
        [Authorize]
        public async Task<IActionResult> GetAllRequestsWithMessages()
        {
            string userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            bool isAdmin = User.IsInRole("Admin");

            var result = await _requestService.GetAllRequestsWithMessagesAsync(userId, isAdmin);

            return Ok(result);
        }

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
                    UserName = createdRequest.User.UserName,
                    UserEmail = createdRequest.User.Email
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
                    UserName = request.User.UserName,
                    UserEmail = request.User.Email
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

                var resultDto = new GetRequestDto
                {
                    IdRequest = request.IdRequest,
                    Description = request.Description,
                    Status = request.Status.ToString(),
                    CreatedAt = request.CreatedAt,
                    UserName = request.User.UserName,
                    UserEmail = request.User.Email
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
