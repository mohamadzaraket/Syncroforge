using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SyncroForge.Hubs
{
    [Authorize]
    public class ChatHub : Hub
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
                Console.WriteLine($"➡️ From user: {Context.UserIdentifier}");

                if (string.IsNullOrEmpty(Context.UserIdentifier))
                {
                    Console.WriteLine("❌ Context.UserIdentifier is null");
                    throw new HubException("User not authenticated");
                }

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
        public async Task AcceptCallUser(string callerUserId, String CallPath)
        {
            Console.WriteLine($"➡️ Call accepted by: {Context.UserIdentifier}");
            Console.WriteLine($"➡️ Sending AcceptCallRequest to caller: {callerUserId}");
            Console.WriteLine($"➡️ Call path: {CallPath}");

            await Clients.User(callerUserId).SendAsync("AcceptCallRequest", new
            {
                callPath = CallPath,
                acceptedBy = Context.UserIdentifier, // The receiver's ID
                callerId = callerUserId // The caller's ID for reference
            });
        }

        public async Task SendWebRTCOffer(string targetUserId, object offer)
        {
            Console.WriteLine($"📤 Sending WebRTC offer from {Context.UserIdentifier} to {targetUserId}");

            if (string.IsNullOrEmpty(targetUserId))
            {
                Console.WriteLine("❌ targetUserId is null or empty in SendWebRTCOffer");
                return;
            }

            await Clients.User(targetUserId).SendAsync("ReceiveWebRTCOffer", new
            {
                offer = offer,
                fromUserId = Context.UserIdentifier
            });
        }

        public async Task SendWebRTCAnswer(string targetUserId, object answer)
        {
            Console.WriteLine($"📤 Sending WebRTC answer from {Context.UserIdentifier} to {targetUserId}");

            if (string.IsNullOrEmpty(targetUserId))
            {
                Console.WriteLine("❌ targetUserId is null or empty in SendWebRTCAnswer");
                return;
            }

            await Clients.User(targetUserId).SendAsync("ReceiveWebRTCAnswer", answer);
        }

        public async Task SendIceCandidate(string targetUserId, object candidate)
        {
            Console.WriteLine($"📤 Sending ICE candidate from {Context.UserIdentifier} to {targetUserId}");

            if (string.IsNullOrEmpty(targetUserId))
            {
                Console.WriteLine("❌ targetUserId is null or empty in SendIceCandidate");
                return;
            }

            await Clients.User(targetUserId).SendAsync("ReceiveIceCandidate", candidate);
        }
    }
}