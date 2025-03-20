using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SyncroForge.Models;
using SyncroForge.Requests.Attendance;
using SyncroForge.Requests.Comany;
using SyncroForge.Requests.Company;
using SyncroForge.Responses;
using SyncroForge.Services.Company;
using System.ComponentModel.Design;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SyncroForge.Controllers
{
   
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _AttendanceService;
        public AttendanceController(IAttendanceService AttendanceService)
        {
            _AttendanceService = AttendanceService;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAttendanceCompany([FromQuery] GetAttendanceCompanyRequest request)
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
                string publicId = jwtToken.Claims.FirstOrDefault(c => c.Type == "publicId")?.Value;
                string userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int userId = int.Parse(userIdString);

                MainResponse response = await _AttendanceService.GetAttendanceCompany(request, publicId, userId);
                return StatusCode(response.Status, response);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(400, new
                {
                    status = 400,
                    message = "error while geting Attendance"
                });
            }

        }

        [HttpPost]
        public async Task<IActionResult> AddAttendance([FromBody] AddAttendanceRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
              

                MainResponse response = await _AttendanceService.AddAttendanceCompany(request);
                return StatusCode(response.Status, response);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(400, new
                {
                    status = 400,
                    message = "error while adding Attendance"
                });
            }

        }
    }
  }