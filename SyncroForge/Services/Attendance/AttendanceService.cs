using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SyncroForge.Data;
using SyncroForge.Models;
using SyncroForge.Requests.Attendance;
using SyncroForge.Responses;
using companny = SyncroForge.Models.Company;



namespace SyncroForge.Services.Company
{
    public class AttendanceService : IAttendanceService
    {
        private readonly AppDbContext _context;

        public AttendanceService(MinioService minioService,AppDbContext context)
        {
        
            _context = context;
        }



        public async Task<MainResponse> GetAttendanceCompany(GetAttendanceCompanyRequest request, string userPublicKey, int userId)
        {
            companny? company =await _context.Companies.Where(i => i.PublicKey == request.CompanyIdentifier && i.IsDeleted == false).FirstOrDefaultAsync();

            if(company == null)
            {
                return new MainResponse()
                {
                    Code = 400,
                    Status = 400,
                    Message = "company not founded",
                    Type = "error",
                    Success = false,
                };
            }

            var Attendances = await _context.Attendances.Include(i=>i.Employee).ThenInclude(j=>j.User).Where(i=>i.Employee.CompanyId== company.Id&& i.IsDeleted==false).Select(i => new
            {
            Email=i.Employee.User.Email,
            FirstName=i.Employee.User.FirstName,
            LastName=i.Employee.User.LastName,
            Logo=i.Employee.User.ProfileUrl,
            AttendanceDate = DateTimeOffset.FromUnixTimeSeconds(i.CreatedAt).UtcDateTime,


        }).ToListAsync();


            return new MainResponse()
            {
                Code = 200,
                Status = 200,
                Message = "Attendances returned successfully",
                Type = "success",
                Success = true,
                data = new
                {
                    Attendances=Attendances,
                }
            };
            


        }

        public async Task<MainResponse> AddAttendanceCompany(AddAttendanceRequest request)
        {
          
            Employee? employee =_context.Employees.Where(i=>i.Id==request.EmployeeId&&i.IsDeleted==false).FirstOrDefault();



            if (employee == null)
            {
                return new MainResponse()
                {
                    Code = 400,
                    Status = 400,
                    Message = "Employee not founded",
                    Type = "error",
                    Success = false,
                };
            }

            Attendance a= new Attendance();
            a.EmployeeId=request.EmployeeId;
            _context.Attendances.Add(a);
            await _context.SaveChangesAsync();

            return new MainResponse()
            {
                Code = 200,
                Status = 200,
                Message = "Attendance Added successfully",
                Type = "success",
                Success = true,
               
            };



        }


    }
}
