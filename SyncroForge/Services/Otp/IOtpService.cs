using SyncroForge.Requests.Otp;
using SyncroForge.Responses.Otp.RegisterResponse;

namespace SyncroForge.Services.Otp
{
    public interface IOtpService
    {
        public  Task<RegisterResponse> Register(RegisterRequest request);
        public Task SendEmail(string title, string value, string toEmail);
    }
}
