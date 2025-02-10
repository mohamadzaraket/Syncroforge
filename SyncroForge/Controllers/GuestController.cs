using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SyncroForge.Requests.Guest;
using SyncroForge.Responses.Guest.RegisterResponse;
using SyncroForge.Services.Guest;

namespace SyncroForge.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GuestController : ControllerBase
    {
        private readonly IGuestService _guestService;
        public GuestController(IGuestService guestService)
        {
            _guestService = guestService;
        }
        [HttpPost]
        public async Task<IActionResult> Regiter(RegisterRequest request)
        {

            RegisterResponse response = await _guestService.Register(request);
            return StatusCode(response.Status, response);

            

        }
        public async Task<IActionResult> Login()
        {

        }

        public async Task<IActionResult> CheckIfEmailAlreadyExist()
        {

        }
    }
}
