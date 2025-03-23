using SyncroForge.Responses;

namespace SyncroForge.Services.AttachmentsService
{
    public interface IAttachmentService
    {
        public Task<MainResponse> DeleteAttachmet(string attachmentPath);
        public Task<MainResponse> ADDAttachmet(IFormFile attachment);
    }
}
