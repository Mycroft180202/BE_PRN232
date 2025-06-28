namespace BE_PRN232.Service
{
    public interface IEmailService
    {
        Task SendEmailVerificationLinkAsync(string toEmail, string userId, string token);
    }
}
