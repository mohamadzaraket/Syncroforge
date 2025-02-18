using SyncroForge.Requests.Comany;
using SyncroForge.Requests.Company;
using SyncroForge.Responses;

namespace SyncroForge.Services.Company
{
    public interface ICompanyService
    {
        public Task<MainResponse> AddCompany(AddCompanyRequest request,string userPublicKey,int userId);
        public Task<MainResponse> GetCompanies(GetCompaniesRequest request, int userId, string userPublicKey);
        public Task<MainResponse> UpdateCompany(UpdateCompanyRequest request, string userPublicKey, int userId);
        public Task<MainResponse> GetCompany(GetCompanyRequest request, String id);
        public Task<MainResponse> InviteUser(InviteUserRequest request, int userId);

        public Task<MainResponse> GetInvitations(GetInvitationsRequest request,String id);

    }
}
