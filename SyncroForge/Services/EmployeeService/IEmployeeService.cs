using SyncroForge.Requests.EmployeeRequests;
using SyncroForge.Responses;

namespace SyncroForge.Services.EmployeeService
{
    public interface IEmployeeService
    {
        public Task<MainResponse> AssignRuleToEmployee(string employeeId,AssignRuleToEmployeeRequest request);
    }
}
