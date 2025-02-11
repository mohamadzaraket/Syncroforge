using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SyncroForge.Requests.Otp;
using SyncroForge.Responses.Otp.RegisterResponse;
using SyncroForge.Services.Otp;

namespace SyncroForge.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OtpController : ControllerBase
    {
        private readonly IOtpService _otpService;
        public OtpController(IOtpService OtpService)
        {
            _otpService = OtpService;
        }
        [HttpPost]
        public async Task<IActionResult> Register([FromBody]RegisterRequest request)
        {
            try
            {

                RegisterResponse response=await _otpService.Register(request);
                return StatusCode(response.Status, response);

                    }catch (Exception ex)
            {

                return StatusCode(400, new
                {
                    status = 400,
                    message = "error while sending otp to register"
                });
            }

        }
    }
}


