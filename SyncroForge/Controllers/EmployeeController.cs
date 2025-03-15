using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SyncroForge.Requests.Comany;
using SyncroForge.Requests.EmployeeRequests;
using SyncroForge.Responses;
using SyncroForge.Services.EmployeeService;

namespace SyncroForge.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }
        [HttpPost("{employeeId}/rule/assign")]
        public async Task<IActionResult> AssignRuleToEmployee(string employeeId, [FromBody] AssignRuleToEmployeeRequest request)
        {
            try
            {
                MainResponse response=await _employeeService.AssignRuleToEmployee(employeeId, request);
                return StatusCode(response.Status, response);

            }catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(400, new
                {
                    status = 400,
                    message = "error while adding company"
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchForEmployee([FromQuery] SearchForEmployeeRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                MainResponse response = await _employeeService.SearchForEmployee(request);
                return StatusCode(response.Status, response);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(400, new
                {
                    status = 400,
                    message = "error while searching employee"
                });
            }

        }

    }
}
