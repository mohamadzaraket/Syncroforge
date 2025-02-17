using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SyncroForge.Models;
using SyncroForge.Requests.Comany;
using SyncroForge.Requests.Department;
using SyncroForge.Responses;
using SyncroForge.Services.Company;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace SyncroForge.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _DepartmentService;
        public DepartmentController(IDepartmentService DepartmentService)
        {
            _DepartmentService = DepartmentService;
        }
        [HttpPost]
        public async Task<IActionResult> AddDepartment([FromForm] AddDepartmentRequest request)
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

                MainResponse response = await _DepartmentService.AddDepartment(request ,userId ,publicId );
                return StatusCode(response.Status, response);

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(400, new
                {
                    status = 400,
                    message = "error while adding department"
                });
            }

        }
        [HttpGet]
        public async Task<IActionResult> GetDepartments([FromQuery] GetDepartmentsRequest request)
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

                MainResponse response = await _DepartmentService.GetDepartments(request, userId, publicId);
                return StatusCode(response.Status, response);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(400, new
                {
                    status = 400,
                    message = "error while adding company"
                });
            }

        }

        [HttpPut]
        public async Task<IActionResult> UpdateDepartment([FromForm] UpdateDepartmentRequest request)
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

                MainResponse response = await _DepartmentService.UpdateDepartment(request, publicId, userId);
                return StatusCode(response.Status, response);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(400, new
                {
                    status = 400,
                    message = "error while Updating company"
                });
            }

        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetDepartment([FromQuery] GetDepartmentRequest request ,string id)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            MainResponse response = await _DepartmentService.GetDepartment(request ,id);
            return StatusCode(response.Status, response);

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return StatusCode(400, new
            {
                status = 400,
                message = "error while adding company"
            });
        }

    }
}
  }