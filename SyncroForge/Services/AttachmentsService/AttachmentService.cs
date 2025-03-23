using Azure.Core;
using SyncroForge.Responses;

namespace SyncroForge.Services.AttachmentsService
{
    public class AttachmentService : IAttachmentService
    {
        private readonly MinioService _minoService;
        public AttachmentService(MinioService minoService) {
            _minoService = minoService;
                }
        public async Task<MainResponse> DeleteAttachmet(string attachmentPath)
        {
            try
            {


                await _minoService.DeleteFileAsync(attachmentPath);
                return new MainResponse()
                {
                    Code = 200,
                    Message = "attachment deleted success",
                    Success = true,
                    Type = "deleted",
                    Status = 200
                };
            }catch(Exception ex)
            {
                throw ex;
            }

            
            
        }
        public async Task<MainResponse> ADDAttachmet(IFormFile attachment)
        {
            try
            {

                string url = $"{Guid.NewGuid().ToString()}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";

                using var stream = attachment.OpenReadStream();
                string path = await _minoService.UploadFileAsync(stream, url);
                return new MainResponse()
                {
                    Code = 200,
                    Message = "attachment Added success",
                    Success = true,
                    Type = "Add",
                    Status = 200,
                    data = new {
                        URL= path,
                    }
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }



        }
    }
}
