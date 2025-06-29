namespace BE_PRN232.Service
{
    public interface IEmailService
    {
        Task SendEmailVerificationLinkAsync(string toEmail, string link);
        Task SendResetPasswordLinkAsync(string toEmail, string link);
        Task SendChangePasswordLinkAsync(string toEmail, string link);

    }
}
