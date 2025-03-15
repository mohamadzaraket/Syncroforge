using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SyncroForge.Models;
using SyncroForge.Requests.Company;
using SyncroForge.Requests.DepartmentEmployee;
using SyncroForge.Responses;
using SyncroForge.Services.Company;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SyncroForge.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DepartmentEmployee : ControllerBase
    {
        private readonly IDepartmentEmployeeService _departmentEmployeeService;
        public DepartmentEmployee(IDepartmentEmployeeService departmentEmployeeService)
        {
            _departmentEmployeeService = departmentEmployeeService;
        }
        [HttpPost]
        public async Task<IActionResult> AssignEmployeeToDepartment([FromBody] DepartmentEmployeeRequest request)
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
                int userId =int.Parse(userIdString);
               
                MainResponse response = await _departmentEmployeeService.AssignEmployeeToDepartment(request, publicId, userId);
                return StatusCode(response.Status, response);

            }
            catch(Exception ex)
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