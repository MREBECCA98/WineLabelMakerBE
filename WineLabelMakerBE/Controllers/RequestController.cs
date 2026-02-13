using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WineLabelMakerBE.Models.DTOs.Requests;
using WineLabelMakerBE.Models.Entity;
using WineLabelMakerBE.Services.Interface;

//The Controller acts as a filter between client requests and server responses
//It receives requests from the client, passes the data to the service that handles the logic,
//and finally returns to the client only the necessary information via DTOs,
//so as not to expose sensitive data
//In this case, it is a Controller for product requests/descriptions
//that the client sends to contact the company

namespace WineLabelMakerBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : ControllerBase
    {

        private readonly IRequestService _requestService;
        private readonly IEmailService _emailService;

        public RequestController(IRequestService requestService, IEmailService emailService)
        {
            _emailService = emailService;
            _requestService = requestService;
        }

        //GET ALL REQUESTS AS NO TRACKING
        //This endpoint returns all requests filtered by user role:
        //-The admin sees all requests from all users
        //-The user sees only their own requests
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
        //This endpoint returns a single request filtered by the user's role:
        //-Admin can see any request
        //-User can only see their own requests
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
        //This endpoint is accessible only to the user
        //Allows the user to modify an existing request
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
        //This endpoint is accessible only to the admin
        //Allows the admin to change the status of the request
        //(pending, in progress, quote sent, payment completed, completed, rejected)
        //And sends a default email based on the changed status
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


                //EMAIL DEFAULT
                //--------------------------------------------------------------------------------------
                string statusIT = request.Status switch
                {
                    RequestStatus.Pending => "In attesa",
                    RequestStatus.InProgress => "In lavorazione",
                    RequestStatus.QuoteSent => "Preventivo inviato",
                    RequestStatus.PaymentConfirmed => "Pagamento confermato",
                    RequestStatus.Completed => "Completata",
                    RequestStatus.Rejected => "Respinta",


                };
                string subject = $"WINE LABEL MAKER - aggiornamento richiesta: {statusIT}";
                string body = request.Status switch
                {


                    RequestStatus.InProgress => $"Gentile {request.User.Name} {request.User.Surname},\n\n" +
                                                $"la sua richiesta con id: {request.IdRequest} è stata presa in carico. \n" +
                                                "La nostra illustratrice ha iniziato a lavorare sulla creazione del prodotto, seguendo le indicazioni da lei fornite.\n\n" +
                                                "A breve riceverà una seconda email con il preventivo per la realizzazione dell’etichetta.\n\n" +
                                                "Grazie per aver scelto Wine Label Maker.",
                    RequestStatus.PaymentConfirmed => $"Gentile {request.User.Name} {request.User.Surname},\n\n" +
                                                      $"abbiamo ricevuto il pagamento relativo alla sua richiesta con id: {request.IdRequest}.\n" +
                                                      "Il nostro team ha iniziato la lavorazione per la sua nuova etichetta di vino e procederà con la creazione secondo le sue indicazioni.\n\n" +
                                                      "A breve riceverà l’email con la presentazione del prodotto in allegato.\n\n" +
                                                      "Grazie per aver scelto Wine Label Maker.",
                    RequestStatus.Rejected => $"Gentile {request.User.Name} {request.User.Surname},\n\n" +
                                              $"Siamo spiacenti di informarla che la sua richiesta con id: {request.IdRequest} per la nuova etichetta di vino, non può essere completata." +
                                              "Se desidera ulteriori dettagli o assistenza, non esiti a contattarci.\n\n" +
                                              "Ci auguriamo di poterla aiutare con altre richieste in futuro.\n\n" +
                                              "Grazie per aver scelto Wine Label Maker.",

                    _ => ""


                };


                //If the body is null, do not send the emai
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
        //This endpoint allows you to delete existing requests based on the user's role:
        //-The admin can delete requests from all users
        //-The user can only delete their own requests
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
