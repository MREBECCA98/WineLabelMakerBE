namespace WineLabelMakerBE.Services.Interface
{
    public interface IEmailService
    {
        //UPDATE ADMIN STATUS
        Task<bool> SendSimpleEmailAsync(string toEmail, string subject, string body);

        //POST FOR COMPLETED BY ID REQUEST
        Task<bool> EmailWithLabelAsync(string toEmail, string subject, string body, string imagePath);
    }
}
