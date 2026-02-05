namespace WineLabelMakerBE.Services.Interface
{
    public interface IEmailService
    {
        //UPDATE ADMIN STATUS
        Task<bool> SendSimpleEmailAsync(string toEmail, string subject, string body);

        //POST FOR COMPLETED, collegato all'id della richiesta
        Task<bool> EmailWithLabelAsync(string toEmail, string subject, string body, string imagePath);
    }
}
