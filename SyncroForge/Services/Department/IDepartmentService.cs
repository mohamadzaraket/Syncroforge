using SyncroForge.Requests.Comany;
using SyncroForge.Requests.Company;
using SyncroForge.Requests.Department;
using SyncroForge.Responses;

namespace SyncroForge.Services.Company
{
    public interface IDepartmentService
    {
        public Task<MainResponse> AddDepartment(AddDepartmentRequest request, int userId, string userPublicKey);
        public Task<MainResponse> GetDepartments(GetDepartmentsRequest request, int userId, string userPublicKey);
        public Task<MainResponse> UpdateDepartment(UpdateDepartmentRequest request, string userPublicKey, int userId);
        public Task<MainResponse> GetDepartment(GetDepartmentRequest request, String id);

    }
}
