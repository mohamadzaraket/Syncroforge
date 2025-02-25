using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace SyncroForge.Hubs
{
    
    [Authorize]
    public class TaskHub:Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Console.WriteLine($"New Connection Assigned To user id:${userId} is ${Context.ConnectionId}");
            await base.OnConnectedAsync();
        }
        public async Task JoinTaskGroup(string TaskIdentifier)
        {
            Console.WriteLine("Joined");
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await Groups.AddToGroupAsync(Context.ConnectionId, TaskIdentifier);
            await Clients.Group(TaskIdentifier).SendAsync("ReceiveNotification", $"User {userId} with connection {Context.ConnectionId} joined task {TaskIdentifier}");
        }
        public async Task LeaveTaskGroup(string TaskIdentifier)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, TaskIdentifier);
            await Clients.Group(TaskIdentifier).SendAsync("ReceiveNotification", $"User {userId} with connection {Context.ConnectionId} left task {TaskIdentifier}");
        }
        public async Task UpdateTask(string TaskIdentifier, object taskData)
        {
            Console.WriteLine($"[DEBUG] Sending update to group: {TaskIdentifier} with data: {taskData}");
            await Clients.Group(TaskIdentifier).SendAsync("TaskUpdated", taskData);
        }

    }
}
