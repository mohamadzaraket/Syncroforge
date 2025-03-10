using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SyncroForge.Requests.User;
using SyncroForge.Responses;
using SyncroForge.Services.User;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SyncroForge.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService) {
            _userService = userService;
        }
        [HttpPost]
        public async Task<IActionResult> JoinCompany(JoinCompanyRequest request)
        {
            try
            {
                string token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                string userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int userId = int.Parse(userIdString);
                string publicId = jwtToken.Claims.FirstOrDefault(c => c.Type == "publicId")?.Value;
                MainResponse response = await _userService.JoinCompany(request, userId, publicId);
                return StatusCode(response.Status, response);


            }
            catch (Exception ex)
            {

                return StatusCode(400, new
                {
                    status = 400,
                    message = "error while joining company"
                });
            }

        }
        [HttpGet]
        public async Task<IActionResult> GetInvitations([FromQuery]GetInvitationsRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                string token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                string userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int userId = int.Parse(userIdString);
                string publicId = jwtToken.Claims.FirstOrDefault(c => c.Type == "publicId")?.Value;
                MainResponse response = await _userService.GetInvitations(request, userId, publicId);
                return StatusCode(response.Status, response);


            }
            catch (Exception ex)
            {

                return StatusCode(400, new
                {
                    status = 400,
                    message = "error while joining company"
                });
            }

        }
        [HttpPost]
        public async Task<IActionResult> ReplyForInvite([FromBody] ReplyForInviteRequest request)
        {
            try
            {
                MainResponse response = await _userService.ReplyForInvite(request);
                return StatusCode(response.Status, response);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(400, new
                {
                    status = 400,
                    message = "error while replying for invite"
                });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                string userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int userId = int.Parse(userIdString);
                MainResponse response = await _userService.GetProfileInfo(userId);
                return StatusCode(response.Status, response);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(400, new
                {
                    status = 400,
                    message = "error while replying for invite"
                });
            }

        }



    }
}
