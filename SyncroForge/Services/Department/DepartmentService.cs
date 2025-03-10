using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SyncroForge.Data;
using SyncroForge.Models;
using SyncroForge.Requests.Comany;
using SyncroForge.Requests.Company;
using SyncroForge.Requests.Department;
using SyncroForge.Responses;
using System.Text.Json;
using companny = SyncroForge.Models.Company;


namespace SyncroForge.Services.Company
{
    public class DepartmentService : IDepartmentService
    {
        private readonly MinioService _minioService;
        private readonly AppDbContext _context;

        public DepartmentService(MinioService minioService,AppDbContext context)
        {
            _minioService = minioService;
            _context = context;
        }
        [HttpPost]
        public async Task<MainResponse> AddDepartment(AddDepartmentRequest request, int userId, string userPublicKey)
        {  
            
            companny? company = await _context.Companies.Where(i=>i.PublicKey==request.CompanyId&&i.IsDeleted==false).FirstOrDefaultAsync();
            if (company == null)
            {
                return new MainResponse()
                {
                    Code = 400,
                    Message = "company not found",
                    Status = 400,
                    Success = false,
                    Type = "not found"
                };

            }

            if (company.CreatedBy != userId)
            {
                return new MainResponse()
                {
                    Code = 400,
                    Message = "Permission denied not company creator ",
                    Status = 400,
                    Success = false,
                    Type = "Permission denied"
                };
            }


            Department? findedDepartment = await _context.Departments.Where(i => i.Name == request.Name && i.IsDeleted == false).FirstOrDefaultAsync();

            if (findedDepartment != null)
            {
                return new MainResponse()
                {
                    Code = 400,
                    Message = "Department name already exists",
                    Status = 400,
                    Success = false,
                    Type = "name conflict"
                };
            }

         

            Department addedDepartment = new Department()
            {
                Name = request.Name,
                CompanyId = company.Id,
                
            };

            

            if (request.Logo != null)
            {
                string baseLogoName = $"{Guid.NewGuid().ToString()}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}_{Path.GetFileName(request.Logo.FileName)}";
                
                using var stream = request.Logo.OpenReadStream();
                string logoPath = await _minioService.UploadFileAsync(stream, $"{userPublicKey}/company/{company.PublicKey}/department/{addedDepartment.PublicKey}/logo/{baseLogoName}");
                addedDepartment.Logo= logoPath;
            }

            await _context.Departments.AddAsync(addedDepartment);
            Count count = await _context.Counts.Where(i => i.Id == 1).FirstOrDefaultAsync();
            Dictionary<String, object> jsonInfo = JsonSerializer.Deserialize<Dictionary<String, object>>(count.JsonInfo);
            if(jsonInfo.ContainsKey($"{company.PublicKey} >> department"))
            {
                jsonInfo[$"{company.PublicKey} >> department"] = ((JsonElement)jsonInfo[$"{company.PublicKey} >> department"]).GetInt32() + 1;
            }
            else
            {
                jsonInfo[$"{company.PublicKey} >> department"] = 1;
            }
            count.JsonInfo = JsonSerializer.Serialize(jsonInfo);

            await _context.SaveChangesAsync();
            return new MainResponse()
            {
                Code = 200,
                Message = "Department added successfully",
                Status = 200,
                Type = "Department Add",
                Success = true,
                data = new
                {
                    identifier = addedDepartment.PublicKey,
                    name = addedDepartment.Name,
                    logoPath = addedDepartment.Logo,
                    Companyidentifier= company.PublicKey,
                }
            };




        }

        public async Task<MainResponse> GetDepartments(GetDepartmentsRequest request, int userId,string userPublicKey)
        {

            companny? company = await _context.Companies.Where(i=>i.PublicKey==request.CompanyId && i.IsDeleted == false).FirstOrDefaultAsync();
            if (company == null)
            {
                return new MainResponse()
                {
                    Code = 400,
                    Message = "company not found",
                    Status = 400,
                    Success = false,
                    Type = "not found"
                };

            }

            if (company.CreatedBy != userId)
            {
                return new MainResponse()
                {
                    Code = 400,
                    Message = "Permission denied not company creator ",
                    Status = 400,
                    Success = false,
                    Type = "Permission denied"
                };
            }


            Count count = await _context.Counts.Where(i => i.Id == 1).FirstOrDefaultAsync();
            Dictionary<String, object> jsonInfo = JsonSerializer.Deserialize<Dictionary<string,object>>(count.JsonInfo);
            int totalDepartments = 0;
            if(jsonInfo.ContainsKey($"{company.PublicKey} >> department")){
                totalDepartments = ((JsonElement)jsonInfo[$"{company.PublicKey} >> department"]).GetInt32();

            }


            var Departments = await _context.Departments.Include(i=>i.DepartmentEmployees).ThenInclude(j=>j.Employee).OrderBy(i => i.Id).Where(j => j.CompanyId== company.Id && j.IsDeleted==false).Skip(request.StartAt).Take(request.Limit).Select(k=>new
            {
                idenitifier = k.PublicKey,
                Name = k.Name,
                logo = k.Logo,
                Emploies=k.DepartmentEmployees.Select(i=> new
                {
                    Employee = i.Employee,
                
                }),
                CreatedAt = k.CreatedAt,
                UpdatedAt = k.UpdatedAt,
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
                    startAt = request.StartAt,
                    limit = request.Limit,
                    totalDepartments = totalDepartments,
                    Departments = Departments
                }
            };
            


        }

        public async Task<MainResponse> UpdateDepartment(UpdateDepartmentRequest request, string userPublicKey, int userId)
        {
            Department? UpdatedDepartment = await _context.Departments.Include(i=>i.Company).Where(i => i.PublicKey==request.Identifier   && i.IsDeleted==false).FirstOrDefaultAsync();
            if(UpdatedDepartment == null)
            {
                return new MainResponse()
                {
                    Code = 400,
                    Message = "Department dose not exist",
                    Status = 400,
                    Success = false,
                    Type = "Not found"
                };
            }

            if(request.Name!= UpdatedDepartment.Name)
            {

        
            Department? findedDepartment = await _context.Departments.Where(i => i.Name == request.Name && i.Company.CreatedBy==userId && i.IsDeleted==false).FirstOrDefaultAsync();

            if (findedDepartment != null)
            {
                return new MainResponse()
                {
                    Code = 400,
                    Message = "Department name already exists",
                    Status = 400,
                    Success = false,
                    Type = "name conflict"
                };
            }
    }




            UpdatedDepartment.Name = request.Name;




            
            if (request.Logo != null)
            {
                string baseLogoName = $"{Guid.NewGuid().ToString()}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}_{Path.GetFileName(request.Logo.FileName)}";
                if (UpdatedDepartment.Logo != null)
                {
          await  _minioService.DeleteFileAsync(UpdatedDepartment.Logo);
                }
                
                using var stream = request.Logo.OpenReadStream();
                string logoPath = await _minioService.UploadFileAsync(stream, $"{userPublicKey}/company/{UpdatedDepartment.Company.PublicKey}/department/{UpdatedDepartment.PublicKey}/logo/{baseLogoName}");
                UpdatedDepartment.Logo = logoPath;
            }
            else
            {
                if (UpdatedDepartment.Logo != null)
                {
                    await _minioService.DeleteFileAsync(UpdatedDepartment.Logo);
                }
                UpdatedDepartment.Logo = null;
            }
            
            await _context.SaveChangesAsync();
           
            return new MainResponse()
            {
                Code = 200,
                Message = "Company updated successfully",
                Status = 200,
                Type = "Company Update",
                Success = true,
                data = new
                {
                    identifier = UpdatedDepartment.PublicKey,
                    name = UpdatedDepartment.Name,
                    logoPath = UpdatedDepartment.Logo,


                }
            };
            

        }

        public async Task<MainResponse> GetDepartment(GetDepartmentRequest request, string id)
        {
            Department? department = null;
        if (request.WithTask && request.WithEmployee)
            {
                department = await _context.Departments.Include(i=>i.DepartmentEmployees).ThenInclude(i=>i.Employee).ThenInclude(i => i.User).Include(i=>i.Tasks).ThenInclude(i=>i.Status).Where(i => i.PublicKey == id && i.IsDeleted == false).FirstOrDefaultAsync();
 
            }else if(request.WithTask && request.WithEmployee==false)
            {
                department = await _context.Departments.Include(i => i.Tasks).ThenInclude(i => i.Status).Where(i => i.PublicKey == id && i.IsDeleted == false).FirstOrDefaultAsync();

            }
            else if(request.WithTask==false && request.WithEmployee )
            {
                department = await _context.Departments.Include(i => i.DepartmentEmployees).ThenInclude(i => i.Employee).ThenInclude(i => i.User).Where(i => i.PublicKey == id && i.IsDeleted == false).FirstOrDefaultAsync();

            }
            else
            {
                department = await _context.Departments.Where(i => i.PublicKey == id && i.IsDeleted == false).FirstOrDefaultAsync();

            }

            if (department == null)
            {
                return new MainResponse()
                {
                    Code = 400,
                    Message = "department dose not exist",
                    Status = 400,
                    Success = false,
                    Type = "Not found"
                };
            }

            return new MainResponse()
            {
                Code = 200,
                Message = "department sent successfully",
                Status = 200,
                Type = "department Found",
                Success = true,
                data = new
                {
                    company = new
                    {
                        department.PublicKey,
                        department.Name,
                        department.Logo,
                      
                       
                        Employees = department.DepartmentEmployees?.Select(e => new
                        {  
                            e.PublicKey,
                            e.Employee.User
                           
                           
                        }),
                        Tasks = department.Tasks?.Select(d => new
                        {
                            d.PublicKey,
                            d.Description,
                            d.ParentTaskId,
                            d.CreatedById,
                            d.Status.Name,
                            
                        }),
                    }
                }
            };


        }
    }
}
