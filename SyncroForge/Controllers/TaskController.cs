using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SyncroForge.Requests.Comany;
using SyncroForge.Requests.Task;
using SyncroForge.Requests.Task;
using SyncroForge.Responses;
using SyncroForge.Services.TaskService;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace SyncroForge.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _TaskService;
        public TaskController(ITaskService TaskService)
        {
            _TaskService = TaskService;
        }
        [HttpPost]
        public async Task<IActionResult> AddTask([FromForm] AddTaskRequest request)
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

                MainResponse response = await _TaskService.AddTask(request ,publicId,userId  );
                return StatusCode(response.Status, response);

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(400, new
                {
                    status = 400,
                    message = "error while adding Task"
                });
            }

        }
        [HttpGet]
        public async Task<IActionResult> GetTasks()
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

                MainResponse response = await _TaskService.GetTasks( userId, publicId);
                return StatusCode(response.Status, response);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(400, new
                {
                    status = 400,
                    message = "error while geting tasks"
                });
            }

        }

        [HttpPut]
        public async Task<IActionResult> UpdateTask([FromForm] UpdateTaskRequest request)
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

                MainResponse response = await _TaskService.UpdateTask(request, publicId, userId);
                return StatusCode(response.Status, response);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(400, new
                {
                    status = 400,
                    message = "error while Updating task"
                });
            }

        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(string id)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
                string userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int userId = int.Parse(userIdString);


                MainResponse response = await _TaskService.GetTask(id, userId);
            return StatusCode(response.Status, response);

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return StatusCode(400, new
            {
                status = 400,
                message = "error while geting task"
            });
        }

    }
}
  }