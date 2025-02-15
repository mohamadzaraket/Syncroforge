
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SyncroForge.Requests.Guest;
using SyncroForge.Services;

namespace SyncroForge.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MinIoTest : ControllerBase
    {
        private readonly MinioService _minioService;
        public MinIoTest(MinioService minioService)
        {
            _minioService = minioService;
        }
        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Invalid file.");
            }

            await using var stream = file.OpenReadStream();
            var fileUrl = await _minioService.UploadFileAsync(stream, file.FileName);

            return Ok(new { Url = fileUrl });
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteFile([FromQuery] TestRequest request)
        {
            await _minioService.DeleteFileAsync(request.fileName);
            return Ok("File deleted successfully.");
        }

    }

}

