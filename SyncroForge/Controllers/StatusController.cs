using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SyncroForge.Responses;
using SyncroForge.Services.StatusService;


namespace SyncroForge.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly IStatusService _statusService;
        public StatusController(IStatusService statusService)
        {
            _statusService = statusService;
        }
        [HttpGet]
        public async Task<IActionResult> GetStatus()
        {
            try
            {
                MainResponse response = await _statusService.GetStatus();
                return StatusCode(response.Status, response);

            }catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(400, new
                {
                    status = 400,
                    message = "error while getting status"
                });
            }
        }
    }
}
