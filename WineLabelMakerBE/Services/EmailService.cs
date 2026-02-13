//Utilizza le classi principali di FluentEmail (IFluentEmail-> metodi da poter usare)
using FluentEmail.Core;
using FluentEmail.Core.Models;
using WineLabelMakerBE.Services.Interface;

namespace WineLabelMakerBE.Services
{
    public class EmailService : IEmailService
    {
        private readonly IFluentEmail _fluentEmail;

        public EmailService(IFluentEmail fluentEmail)
        {
            _fluentEmail = fluentEmail;
        }

        //It is used within the RequestController when the state changes to send
        //The default email with its body
        public async Task<bool> SendSimpleEmailAsync(string toEmail, string subject, string body)
        {
            var response = await _fluentEmail
                .To(toEmail)
                .Subject(subject)
                .Body(body, isHtml: false)
                .SendAsync();

            return response.Successful;
        }

        //Email with added label image for "Completed" status
        public async Task<bool> EmailWithLabelAsync(string toEmail, string subject, string body, string imagePath)
        {
            if (!File.Exists(imagePath))
                return false;

            var response = await _fluentEmail
                .To(toEmail)
                .Subject(subject)
                .Body(body, isHtml: false)
                .Attach(new Attachment
                {
                    Filename = Path.GetFileName(imagePath),
                    Data = new MemoryStream(await File.ReadAllBytesAsync(imagePath)),
                    ContentType = "image/png"
                })
                .SendAsync();

            return response.Successful;
        }
    }
}


