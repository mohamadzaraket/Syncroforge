using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SyncroForge.Models;
using SyncroForge.Requests.Guest;
using SyncroForge.Responses.Guest.LoginResponse;
using SyncroForge.Responses.Guest.RegisterResponse;
using SyncroForge.Services.Guest;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
        public async Task<IActionResult> Register([FromBody]RegisterRequest request)
        {
            try
            {

                RegisterResponse response = await _guestService.Register(request);
                return StatusCode(response.Status, response);

            }catch(Exception ex)
            {
                return StatusCode(400, new
                {
                    status = 400,
                    message = "error while trying to register"
                });
            }

            

        }
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {

                LoginResponse response = await _guestService.Login(request);
                return StatusCode(response.Status, response);

            }
            catch (Exception ex)
            {
                return StatusCode(400, new
                {
                    status = 400,
                    message = "error while trying to login"
                });
            }



        }

        /*public async Task<IActionResult> Login()
        {
            return

        }

        public async Task<IActionResult> CheckIfEmailAlreadyExist()
        {

        }*/

    }
}
