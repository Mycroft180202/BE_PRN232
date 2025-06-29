namespace BE_PRN232.RequestDTO
{
    public class ResetPasswordRequest
    {
        public Guid UserId { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
