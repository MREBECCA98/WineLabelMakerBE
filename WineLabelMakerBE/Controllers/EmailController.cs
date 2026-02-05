using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WineLabelMakerBE.Models.DTOs.Email;
using WineLabelMakerBE.Models.Entity;
using WineLabelMakerBE.Services.Interface;

//Questo controller è accessibile solo all'Admin.
//Permette di inviare l'email relativa a una richiesta specifica quando lo stato è "Completed",
//allegando l'immagine dell'etichetta creata.
namespace WineLabelMakerBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class EmailController : ControllerBase
    {
        //Iniezione dell'interfaccia per la gestione delle email in base alla richiesta
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

        //POST FOR COMPLETED, collegato all'id della richiesta
        [HttpPost("completed")]
        public async Task<IActionResult> SendEmail(EmailRequestDto dto)
        {
            if (!ModelState.IsValid)

                return BadRequest(ModelState);
            //ID della RICHIESTA
            var request = await _requestService.GetRequestsByIdAsync(dto.RequestId);
            if (request == null)
                return NotFound("Richiesta non trovata.");

            if (request.Status != RequestStatus.Completed)
                return BadRequest("L'email con allegato può essere inviata solo per richieste completate.");

            //Email da inviare a chi ha fatto la richiesta 
            string toEmail = request.User.Email;

            //Traduzione dell'enum
            string statusIT = request.Status switch
            {
                RequestStatus.Completed => "Completata",

            };

            //Subject e body default per l'email status "Completed"
            string subject = $"WINE LABEL MAKER - aggiornamento richiesta: {statusIT}";
            string body = !string.IsNullOrWhiteSpace(dto.CustomBody) ?
                          dto.CustomBody : $"Gentile {request.User.Name} {request.User.Surname},\n\n" +
                                           "Siamo felici di informarla che la sua richiesta per la nuova etichetta di vino è stata completata con successo. " +
                                           "La nostra illustratrice ha realizzato l’etichetta seguendo la descrizione da lei fornita.\n\n" +
                                           "Troverà l’etichetta in allegato a questa email.\n\n" +
                                           "Per qualsiasi chiarimento, modifica o ulteriore richiesta, non esiti a contattarci: " +
                                           "saremo lieti di assisterla e di mettere la nostra esperienza a sua disposizione.\n\n" +
                                           "Cordiali saluti,\n" +
                                           "Il team di Wine Label Maker";


            //Immagine etichetta
            if (string.IsNullOrEmpty(dto.ImageName))
                return BadRequest("Devi fornire il nome dell'immagine da allegare.");

            string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "labels", dto.ImageName);
            if (!System.IO.File.Exists(imagePath))
                return NotFound($"Immagine {dto.ImageName} non trovata.");

            //Mail con allegato
            bool invioOk = await _emailService.EmailWithLabelAsync(toEmail, subject, body, imagePath);

            return invioOk ? Ok("Mail inviata con successo!") : StatusCode(500, "Errore nell'invio della mail");
        }

        //Aggiunta immagina FE
        [HttpPost("uploadLabel")]
        public async Task<IActionResult> UploadLabel(IFormFile labelImage)
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
    }

}




