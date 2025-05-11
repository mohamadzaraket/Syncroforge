using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SyncroForge.Hubs
{
    [Authorize]
    public class ChatHub:Hub
    {
        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"✅ User connected: {Context.UserIdentifier}");

            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"✅ User disconnected: {Context.UserIdentifier}");
            return base.OnDisconnectedAsync(exception);
        }
        
        public async Task RequestToCallUser(string targetUserId, string CallPath)
        {
            try
            {
                Console.WriteLine($"➡️ Calling userId: {targetUserId} with path: {CallPath}");
                Console.WriteLine($"➡️ Context.UserIdentifier: {Context.UserIdentifier}");

                // Validate current user
                if (string.IsNullOrEmpty(Context.UserIdentifier))
                {
                    Console.WriteLine("❌ Context.UserIdentifier is null");
                    throw new HubException("User not authenticated");
                }

                // Validate target user
                if (string.IsNullOrEmpty(targetUserId))
                {
                    Console.WriteLine("❌ targetUserId is empty");
                    throw new HubException("Invalid target user ID");
                }

                await Clients.User(targetUserId).SendAsync("ReceiveCallRequest", new
                {
                    callPath = CallPath,
                    fromUser = Context.UserIdentifier
                });

                Console.WriteLine("✅ Call request sent.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error in RequestToCallUser: " + ex.Message);
                throw;
            }
        }

        public async Task AcceptCallUser(string targetUserId, String CallPath)
        {
            Console.WriteLine($"➡️ accepting userId: {targetUserId} with path: {CallPath}");
            await Clients.User(targetUserId).SendAsync("AcceptCallRequest", new
            {
                callPath = CallPath
            });
        }
    }
}
