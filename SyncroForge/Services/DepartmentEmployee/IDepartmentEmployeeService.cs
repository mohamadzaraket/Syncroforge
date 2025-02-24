using SyncroForge.Requests.Comany;
using SyncroForge.Requests.Company;
using SyncroForge.Requests.DepartmentEmployee;
using SyncroForge.Responses;

namespace SyncroForge.Services.Company
{
    public interface IDepartmentEmployeeService
    {
        public Task<MainResponse> AssignEmployeeToDepartment(DepartmentEmployeeRequest request,string userPublicKey,int userId);
    

    }
}
