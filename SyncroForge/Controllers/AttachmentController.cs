using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SyncroForge.Responses;
using SyncroForge.Services.AttachmentsService;

namespace SyncroForge.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class AttachmentController : ControllerBase
    {
        private readonly IAttachmentService _attachmentService;
       
            public AttachmentController(IAttachmentService attachmentService)
        {
            _attachmentService = attachmentService;

        }
        [HttpDelete]
        public async Task<IActionResult> DeleteAttachmet([FromQuery] string attachmentPath)
        {
            try
            {
                MainResponse response=await _attachmentService.DeleteAttachmet(attachmentPath);
                return StatusCode(response.Status, response);

            }catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(400, new
                {
                    status = 400,
                    message = "error while deleting attachment"
                });
            }

        }
    }
}
