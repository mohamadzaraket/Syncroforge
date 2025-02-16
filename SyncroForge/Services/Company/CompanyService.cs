using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SyncroForge.Data;
using SyncroForge.Models;
using SyncroForge.Requests.Comany;
using SyncroForge.Requests.Company;
using SyncroForge.Responses;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using companny = SyncroForge.Models.Company;


namespace SyncroForge.Services.Company
{
    public class CompanyService:ICompanyService
    {
        private readonly MinioService _minioService;
        private readonly AppDbContext _context;

        public CompanyService(MinioService minioService,AppDbContext context)
        {
            _minioService = minioService;
            _context = context;
        }
        [HttpPost]
        public async Task<MainResponse> AddCompany(AddCompanyRequest request,string userPublicKey,int userId)
        {
            companny findedCompany = await _context.Companies.Where(i => i.Name == request.Name).FirstOrDefaultAsync();

            if (findedCompany != null)
            {
                return new MainResponse()
                {
                    Code = 400,
                    Message = "company name already exists",
                    Status = 400,
                    Success = false,
                    Type = "name conflict"
                };
            }
            companny addedCompany = new companny()
            {
                Name = request.Name,
                Description = request.Description,
                CreatedBy= userId
            };

            

            if (request.CompanyLogo != null)
            {
                string baseLogoName = $"{Guid.NewGuid().ToString()}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}_{Path.GetFileName(request.CompanyLogo.FileName)}";
                
                using var stream = request.CompanyLogo.OpenReadStream();
                string logoPath = await _minioService.UploadFileAsync(stream, $"{userPublicKey}/company/{addedCompany.PublicKey}/logo/{baseLogoName}");
                addedCompany.Logo_Url= logoPath;
            }

            await _context.Companies.AddAsync(addedCompany);
            Count count = await _context.Counts.Where(i => i.Id == 1).FirstOrDefaultAsync();
            Dictionary<String, object> jsonInfo = JsonSerializer.Deserialize<Dictionary<String, object>>(count.JsonInfo);
            if(jsonInfo.ContainsKey($"{userPublicKey} >> company"))
            {
                jsonInfo[$"{userPublicKey} >> company"] = ((JsonElement)jsonInfo[$"{userPublicKey} >> company"]).GetInt32() + 1;
            }
            else
            {
                jsonInfo[$"{userPublicKey} >> company"] = 1;
            }
            count.JsonInfo = JsonSerializer.Serialize(jsonInfo);

            await _context.SaveChangesAsync();
            return new MainResponse()
            {
                Code = 200,
                Message = "Company added successfully",
                Status = 200,
                Type = "Company Add",
                Success = true,
                data = new
                {
                    identifier = addedCompany.PublicKey,
                    name = addedCompany.Name,
                    description = addedCompany.Description,
                    logoPath = addedCompany.Logo_Url,
                    

                }
            };




        }

        public async Task<MainResponse> GetCompanies(GetCompaniesRequest request, int userId,string userPublicKey)
        {
            Count count = await _context.Counts.Where(i => i.Id == 1).FirstOrDefaultAsync();
            Dictionary<String, object> jsonInfo = JsonSerializer.Deserialize<Dictionary<string,object>>(count.JsonInfo);
            int totalCompanies = 0;
            if(jsonInfo.ContainsKey($"{userPublicKey} >> company")){
                totalCompanies = ((JsonElement)jsonInfo[$"{userPublicKey} >> company"]).GetInt32();

            }


            var companies = await _context.Companies.OrderBy(i => i.Id).Where(j => j.CreatedBy == userId && j.IsDeleted==false).Skip(request.StartAt).Take(request.Limit).Select(k=>new
            {
                idenitifier = k.PublicKey,
                Name = k.Name,
                Description = k.Description,
                logoUrl = k.Logo_Url,
                CreatedAt = k.CreatedAt,
                UpdatedAt = k.UpdatedAt
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
                    totalCompanies = totalCompanies,
                    companies = companies
                }
            };
            


        }

        public async Task<MainResponse> UpdateCompany(UpdateCompanyRequest request, string userPublicKey, int userId)
        {
     companny? UpdatedCompany= await _context.Companies.Where(i => i.PublicKey==request.publicCompanyId && i.CreatedBy == userId && i.IsDeleted==false).FirstOrDefaultAsync();
            if(UpdatedCompany == null)
            {
                return new MainResponse()
                {
                    Code = 400,
                    Message = "company dose not exist",
                    Status = 400,
                    Success = false,
                    Type = "Not found"
                };
            }


            companny? findedCompany = await _context.Companies.Where(i => i.Name == request.Name && i.CreatedBy!=userId && i.IsDeleted==false).FirstOrDefaultAsync();

            if (findedCompany != null)
            {
                return new MainResponse()
                {
                    Code = 400,
                    Message = "company name already exists",
                    Status = 400,
                    Success = false,
                    Type = "name conflict"
                };
            }

       



            UpdatedCompany.Name = request.Name;
            UpdatedCompany.Description = request.Description;




            
            if (request.CompanyLogo != null)
            {
                string baseLogoName = $"{Guid.NewGuid().ToString()}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}_{Path.GetFileName(request.CompanyLogo.FileName)}";
                if (UpdatedCompany.Logo_Url != null)
                {
          await  _minioService.DeleteFileAsync(UpdatedCompany.Logo_Url);
                }
                
                using var stream = request.CompanyLogo.OpenReadStream();
                string logoPath = await _minioService.UploadFileAsync(stream, $"{userPublicKey}/company/{UpdatedCompany.PublicKey}/logo/{baseLogoName}");
                UpdatedCompany.Logo_Url = logoPath;
            }
            else
            {
                if (UpdatedCompany.Logo_Url != null)
                {
                    await _minioService.DeleteFileAsync(UpdatedCompany.Logo_Url);
                }
                UpdatedCompany.Logo_Url = null;
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
                    identifier = UpdatedCompany.PublicKey,
                    name = UpdatedCompany.Name,
                    description = UpdatedCompany.Description,
                    logoPath = UpdatedCompany.Logo_Url,


                }
            };
            

        }

        public async Task<MainResponse> GetCompany(GetCompanyRequest request, string id)
        {
            companny? company=null;
        if (request.WithDepartment && request.WithEmployee)
            {
              company = await _context.Companies.Include(i=>i.Employees).Include(i=>i.Departments).Where(i => i.PublicKey == id && i.IsDeleted == false).FirstOrDefaultAsync();
 
            }else if(request.WithDepartment && request.WithEmployee==false)
            {
                company = await _context.Companies.Include(i => i.Departments).Where(i => i.PublicKey == id && i.IsDeleted == false).FirstOrDefaultAsync();

            }
            else if(request.WithDepartment==false && request.WithEmployee )
            {
                company = await _context.Companies.Include(i => i.Employees).Where(i => i.PublicKey == id && i.IsDeleted == false).FirstOrDefaultAsync();

            }
            else
            {
                company = await _context.Companies.Where(i => i.PublicKey == id && i.IsDeleted == false).FirstOrDefaultAsync();

            }

            if (company == null)
            {
                return new MainResponse()
                {
                    Code = 400,
                    Message = "company dose not exist",
                    Status = 400,
                    Success = false,
                    Type = "Not found"
                };
            }

            return new MainResponse()
            {
                Code = 200,
                Message = "Company sent successfully",
                Status = 200,
                Type = "Company Found",
                Success = true,
                data = new
                {
                    company = new
                    {
                        company.PublicKey,
                        company.Name,
                        company.Logo_Url,
                        company.Description,
                       
                        Employees = company.Employees?.Select(e => new
                        {  
                            e.PublicKey,
                            e.User.Email,
                            e.User.FirstName,
                            e.User.LastName,
                           
                           
                        }),
                        Departments = company.Departments?.Select(d => new
                        {
                            d.PublicKey,
                            d.Name,
                            d.Logo,
                        }),
                    }
                }
            };


        }
    }
}
