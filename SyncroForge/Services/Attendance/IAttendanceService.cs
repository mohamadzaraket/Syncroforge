using SyncroForge.Requests.Attendance;
using SyncroForge.Requests.Comany;
using SyncroForge.Requests.Company;
using SyncroForge.Responses;

namespace SyncroForge.Services.Company
{
    public interface IAttendanceService
    {
        public Task<MainResponse> GetAttendanceCompany(GetAttendanceCompanyRequest request,string userPublicKey,int userId);

        public Task<MainResponse> AddAttendanceCompany(AddAttendanceRequest request);
    }
}
