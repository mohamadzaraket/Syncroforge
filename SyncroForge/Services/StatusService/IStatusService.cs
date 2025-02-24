using SyncroForge.Responses;

namespace SyncroForge.Services.StatusService
{
    public interface IStatusService
    {
        public Task<MainResponse> GetStatus();
    }
}
