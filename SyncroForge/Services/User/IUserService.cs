using SyncroForge.Requests.User;
using SyncroForge.Responses;

namespace SyncroForge.Services.User
{
    public interface IUserService
    {
        public Task<MainResponse> JoinCompany(JoinCompanyRequest request,int userId, string publicUserId);
        public Task<MainResponse> GetInvitations(GetInvitationsRequest request, int userId, string publicUserId);
        public Task<MainResponse> ReplyForInvite(ReplyForInviteRequest request);
        public Task<MainResponse> GetProfileInfo(int userId);
    }
}
