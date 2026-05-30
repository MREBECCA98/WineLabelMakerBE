using Resend;
using WineLabelMakerBE.Services.Interface;

namespace WineLabelMakerBE.Services
{
    public class EmailService : IEmailService
    {
        private readonly IResend _resend;

        public EmailService(IResend resend)
        {
            _resend = resend;
        }

        public async Task<bool> SendSimpleEmailAsync(string toEmail, string subject, string body)
        {
            var message = new EmailMessage();
            message.From = "Wine Label Maker <onboarding@resend.dev>";
            message.To.Add(toEmail);
            message.Subject = subject;
            message.TextBody = body;

            var response = await _resend.EmailSendAsync(message);
            return response != null;
        }

        public async Task<bool> EmailWithLabelAsync(string toEmail, string subject, string body, string imagePath)
        {
            if (!File.Exists(imagePath)) return false;

            var message = new EmailMessage();
            message.From = "Wine Label Maker <onboarding@resend.dev>";
            message.To.Add(toEmail);
            message.Subject = subject;
            message.TextBody = body;

            var response = await _resend.EmailSendAsync(message);
            return response != null;
        }
    }
}