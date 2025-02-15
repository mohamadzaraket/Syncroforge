using SyncroForge.Requests.Company;
using SyncroForge.Responses;

namespace SyncroForge.Services.Company
{
    public interface ICompanyService
    {
        public Task<MainResponse> AddCompany(AddCompanyRequest request,string userPublicKey,int userId);
    }
}
