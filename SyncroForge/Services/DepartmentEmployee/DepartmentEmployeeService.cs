using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SyncroForge.Data;
using SyncroForge.Models;
using SyncroForge.Requests.DepartmentEmployee;
using SyncroForge.Responses;



namespace SyncroForge.Services.Company
{
    public class departmentEmployeeService : IDepartmentEmployeeService
    {
     
        private readonly AppDbContext _context;

        public departmentEmployeeService(AppDbContext context)
        {
          
            _context = context;
        }
       
        public async Task<MainResponse> AssignEmployeeToDepartment(DepartmentEmployeeRequest request,string userPublicKey,int userId)
        {
           
         
            Department? department=await _context.Departments.Include(i=>i.Company).Where(i=>i.PublicKey==request.DepartmentIdentifier && i.IsDeleted==false).FirstOrDefaultAsync();
            if (department == null)
            {
                return new MainResponse()
                {
                    Code = 400,
                    Message = "department not found",
                    Status = 400,
                    Success = false,
                    Type = "not found"
                };
            }
            if(department.Company.CreatedBy!=userId) {
                return new MainResponse()
                {
                    Code = 400,
                    Message = "No permission  not company creator",
                    Status = 400,
                    Success = false,
                    Type = "permission denied "
                };
            }


            Employee? employee = await _context.Employees.Where(i => i.PublicKey == request.EmployeeIdentifier && i.IsDeleted==false).FirstOrDefaultAsync();
            if(employee == null)
            {
                return new MainResponse()
                {
                    Code = 400,
                    Message = "Employee not found",
                    Status = 400,
                    Success = false,
                    Type = "not found"
                };
            }
            DepartmentEmployee? departmentEmployee = await _context.DepartmentEmployees.Where(i => i.DepartmentId == department.Id && i.EmployeeId == employee.Id && i.IsDeleted == false).FirstOrDefaultAsync();

            if (departmentEmployee != null)
            {
                return new MainResponse()
                {
                    Code = 400,
                    Message = "Employee  already in this department",
                    Status = 400,
                    Success = false,
                    Type = "conflict"
                };
            }

            await _context.DepartmentEmployees.AddAsync(new DepartmentEmployee
            {
                DepartmentId = department.Id,
                EmployeeId = employee.Id,
            });

            await _context.SaveChangesAsync();

            return new MainResponse()
            {
                Code = 200,
                Message = "Employee Assign to Department  successfully",
                Status = 200,
                Success = true,
                Type = "success"
            };

        }
    }
}
