using Microsoft.EntityFrameworkCore;
using SyncroForge.Data;
using SyncroForge.Models;
using SyncroForge.Requests.Comany;
using SyncroForge.Requests.EmployeeRequests;
using SyncroForge.Responses;

namespace SyncroForge.Services.EmployeeService
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _context;
        public EmployeeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<MainResponse> AssignRuleToEmployee(string employeeId,AssignRuleToEmployeeRequest request)
        {

            Employee employee = await _context.Employees.Where(i => i.PublicKey == employeeId).FirstOrDefaultAsync();
            if(employee == null)
            {
                return new MainResponse()
                {
                    Code = 400,
                    Status = 400,
                    Message = "Employee not found",
                    Success = false,
                    Type = "Conflict"

                };
            }
            Rule rule = await _context.Rule.Where(i => i.PublicKey == request.RuleId).FirstOrDefaultAsync();

            if (rule == null)
            {
                return new MainResponse()
                {
                    Code = 400,
                    Status = 400,
                    Message = "rule not found",
                    Success = false,
                    Type = "Conflict"

                };
            }
            employee.RuleId = rule.Id;
            await _context.SaveChangesAsync();
            return new MainResponse()
            {
                Code = 200,
                Status = 200,
                Message = "rule is assigned successfully",
                Success = true,
                Type = "success",
                data = new
                {
                    ruleName = rule.RuleName,
                    ruleIdentifier = rule.PublicKey
                }
            };




        }

        public async Task<MainResponse> SearchForEmployee(SearchForEmployeeRequest request)
        {
            var employees = await _context.Employees.Include(i=>i.User).Where(i => i.User.Email.Contains(request.Email)).Select(k => new
            {
                idenitifier = k.PublicKey,
                Name = k.User.Email,
              
               
            }).ToListAsync();
            return new MainResponse()
            {
                Code = 200,
                Status = 200,
                Message = "companies returned successfully",
                Type = "success",
                Success = true,
                data = new
                {
                    employees = employees
                }
            };
        }
    }
}
