using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WineLabelMakerBE.Models.DTOs.Email;
using WineLabelMakerBE.Models.Entity;
using WineLabelMakerBE.Services.Interface;

//This controller is accessible only to Admins.
//It allows sending an email for a specific request when the status is "Completed",
//attaching the created label image.
//It also allows sending a custom email when the status is "QuoteSent" to deliver the quote.

namespace WineLabelMakerBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly IRequestService _requestService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EmailController(
            IEmailService emailService,
            IRequestService requestService,
            IWebHostEnvironment webHostEnvironment)
        {
            _emailService = emailService;
            _requestService = requestService;
            _webHostEnvironment = webHostEnvironment;
        }

        //POST FOR COMPLETED BY REQUEST ID
        [HttpPost("completed")]
        public async Task<IActionResult> SendEmail(EmailRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                //Request ID
                var request = await _requestService.GetRequestsByIdAsync(dto.RequestId);
                if (request == null)
                    return NotFound("Richiesta non trovata.");

                if (request.Status != RequestStatus.Completed)
                    return BadRequest("L'email con allegato può essere inviata solo per richieste completate.");


                string toEmail = request.User.Email;

                //Enum
                string statusIT = request.Status switch
                {
                    RequestStatus.Completed => "Completata",

                };

                //Subject e body default for "Completed" email
                string subject = $"WINE LABEL MAKER - aggiornamento richiesta: {statusIT}";
                string body = !string.IsNullOrWhiteSpace(dto.CustomBody) ?
                              dto.CustomBody : $"Gentile {request.User.Name} {request.User.Surname},\n\n" +
                                               $"siamo felici di informarla che la sua richiesta con id: {request.IdRequest} è stata completata con successo.\n" +
                                               "La nostra illustratrice, ha realizzato l’etichetta, che trova in allegato, seguendo la descrizione da lei fornita.\n\n" +
                                               "Per qualsiasi chiarimento, modifica o ulteriore richiesta, non esiti a contattarci;" +
                                               "saremo lieti di assisterla e di mettere la nostra esperienza a sua disposizione.\n\n" +
                                               "Cordiali saluti.\n" +
                                               "Il team di Wine Label Maker";


                //Label image
                if (string.IsNullOrEmpty(dto.ImageName))
                    return BadRequest("Devi fornire il nome dell'immagine da allegare.");

                string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "labels", dto.ImageName);
                if (!System.IO.File.Exists(imagePath))
                    return NotFound($"Immagine {dto.ImageName} non trovata.");

                //Mail with imgage
                bool invioOk = await _emailService.EmailWithLabelAsync(toEmail, subject, body, imagePath);

                return invioOk ? Ok("Mail inviata con successo!") : StatusCode(500, "Errore nell'invio della mail");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Errore interno: {ex.Message}");
            }
        }

        //Adding Front End Image
        [HttpPost("uploadLabel")]
        public async Task<IActionResult> UploadLabel(IFormFile labelImage)
        {
            try
            {
                if (labelImage == null || labelImage.Length == 0)
                    return BadRequest("Nessun file selezionato.");

                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "labels", labelImage.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await labelImage.CopyToAsync(stream);
                }

                return Ok("Immagine caricata con successo!");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Errore interno: {ex.Message}");

            }

        }


        //POST FOR QUOTE-SENT - linked to the request ID
        //Used to send the personalized email for the quote request
        [HttpPost("sendQuote")]
        public async Task<IActionResult> SendQuoteEmail(EmailQuoteDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                //Request ID
                var request = await _requestService.GetRequestsByIdAsync(dto.RequestId);
                if (request == null)
                    return NotFound("Richiesta non trovata.");

                if (request.Status != RequestStatus.QuoteSent)
                    return BadRequest("L'email del preventivo può essere inviata solo per richieste con status 'QuoteSent'.");

                //Email to be sent to the person who made the request
                string toEmail = request.User.Email;

                //Enum
                string statusIT = request.Status switch
                {
                    RequestStatus.QuoteSent => "Preventivo inviato",

                };

                string subject = $"WINE LABEL MAKER - aggiornamento richiesta: {statusIT}";


                string body = dto.CustomBody;

                bool invioOk = await _emailService.SendSimpleEmailAsync(toEmail, subject, body);

                return invioOk ? Ok("Email del preventivo inviata con successo!")
                                : StatusCode(500, "Errore nell'invio della mail");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Errore interno: {ex.Message}");
            }
        }
    }
}




