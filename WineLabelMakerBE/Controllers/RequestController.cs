using Microsoft.AspNetCore.Mvc;
using WineLabelMakerBE.Models.DTOs.Requests;
using WineLabelMakerBE.Models.Entity;
using WineLabelMakerBE.Services.Interface;

namespace WineLabelMakerBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        private readonly IRequestService _requestService;

        public RequestController(IRequestService requestService)
        {
            _requestService = requestService;
        }

        //GET ALL REQUESTS AS NO TRACKING
        [HttpGet("allRequest")]
        public async Task<ActionResult<IEnumerable<GetRequestDto>>> GetAllRequests()
        {
            try
            {
                var requests = await _requestService.GetAllRequestsAsync();
                var requestDto = requests.Select(r => new GetRequestDto
                {
                    IdRequest = r.IdRequest,
                    Description = r.Description,
                    Status = r.Status,
                    CreatedAt = r.CreatedAt,
                    UserName = r.User.UserName,
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
        [HttpGet("requestById/{id:guid}")]
        public async Task<ActionResult<GetRequestDto>> GetRequestById(Guid id)
        {
            try
            {
                if (Guid.Empty == id)
                    return BadRequest("Id non valido");

                var request = await _requestService.GetRequestsByIdAsync(id);

                if (request == null)
                    return NotFound();

                var requestDto = new GetRequestDto
                {
                    IdRequest = request.IdRequest,
                    Description = request.Description,
                    Status = request.Status,
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

        //GET SEARCH AS NO TRACKING
        [HttpGet("searchRequest/{searchTerm}")]
        public async Task<ActionResult<IEnumerable<GetRequestDto>>> GetSearchRequest(string searchTerm)
        {
            try
            {
                var requests = await _requestService.GetRequestSearchAsync(searchTerm);

                var result = requests.Select(r => new GetRequestDto
                {
                    IdRequest = r.IdRequest,
                    Description = r.Description,
                    Status = r.Status,
                    CreatedAt = r.CreatedAt,
                    UserName = r.User.UserName,
                    UserEmail = r.User.Email
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(
                   StatusCodes.Status500InternalServerError, $"Errore interno: {ex.Message}");
            }
        }

        //POST CREATE REQUEST
        [HttpPost("createRequest")]
        public async Task<IActionResult> CreateRequest(CreateRequestDto requestDto)
        {
            try
            {
                string userId = "test-user-id";
                var createdRequest = await _requestService.CreateRequestAsync(requestDto, userId);

                var resultDto = new GetRequestDto
                {
                    IdRequest = createdRequest.IdRequest,
                    Description = createdRequest.Description,
                    Status = createdRequest.Status,
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
        [HttpPut("updateClient/{id:guid}")]
        public async Task<IActionResult> UpdateRequestDescription(Guid id, UpdateRequestDescriptionDto descriptionDto)
        {

            try
            {
                if (descriptionDto == null || string.IsNullOrWhiteSpace(descriptionDto.Description))
                    return BadRequest("Descrizione non valida");

                var request = await _requestService.GetRequestsByIdAsync(id);
                if (request == null)
                    return NotFound("Richiesta non trovata");

                request.Description = descriptionDto.Description;
                request.UpdatedAt = DateTime.UtcNow;

                bool saved = await _requestService.Save();
                return saved ? Ok(request) : BadRequest("Impossibile aggiornare la richiesta");
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError, $"Errore interno: {ex.Message}");
            }
        }

        //AGGIUNGERE AUTORIZZAZIONE RUOLI SOLO PER GLI ADMIN
        //UPDATE ADMIN STATUS
        [HttpPut("updateAdmin/{id:guid}")]
        public async Task<IActionResult> UpdateRequestStatus(Guid id, UpdateRequestStatusDto StatusDto)
        {
            try
            {
                if (StatusDto == null)
                    return BadRequest("Dati non validi");

                var request = await _requestService.GetRequestsByIdAsync(id);
                if (request == null)
                    return NotFound("Richiesta non trovata");

                request.Status = StatusDto.Status;
                request.UpdatedAt = DateTime.UtcNow;

                bool saved = await _requestService.Save();
                return saved ? Ok(request) : BadRequest("Impossibile aggiornare lo stato");
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError, $"Errore interno: {ex.Message}");
            }
        }


        //AGGIUNGERE AUTORIZZAZIONE RUOLI SOLO PER GLI ADMIN
        //DELETE REQUEST
        [HttpDelete("deleteRequest/{id:guid}")]
        public async Task<IActionResult> DeleteRequest(Guid id)
        {
            try
            {
                if (Guid.Empty == id)
                    return BadRequest("Id non valido");

                var request = await _requestService.GetRequestsById(id);
                if (request == null)
                    return NotFound("Richiesta non trovata");

                bool result = await this._requestService.DeleteRequestAsync(id);
                return result ? Ok("Richiesta eliminata con successo") : BadRequest("Impossibile eliminare la richiesta");
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError, $"Errore interno: {ex.Message}");
            }
        }
    }
}
