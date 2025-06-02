using SyncroForge.Requests.Task;
using SyncroForge.Responses;

namespace SyncroForge.Services.TaskService
{
    public interface ITaskService
    {
        public Task<MainResponse> AddTask(AddTaskRequest request, string userPublicKey, int userId);
        public Task<MainResponse> GetTasks( int userId, string userPublicKey);
        public Task<MainResponse> UpdateTask(UpdateTaskRequest request, string userPublicKey, int userId);
        public Task<MainResponse> GetTask(string id,int userId);
    }
}
