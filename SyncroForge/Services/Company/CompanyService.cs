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
using userr = SyncroForge.Models.User;


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


          //  if (UpdatedCompany.Name==request.Name && UpdatedCompany.Description == request.Description && UpdatedCompany.Logo_Url==request.CompanyLogo)
           
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
              company = await _context.Companies.Include(i=>i.Employees).ThenInclude(i=>i.User).Include(i=>i.Departments).Where(i => i.PublicKey == id && i.IsDeleted == false).FirstOrDefaultAsync();
 
            }else if(request.WithDepartment && request.WithEmployee==false)
            {
                company = await _context.Companies.Include(i => i.Departments).Where(i => i.PublicKey == id && i.IsDeleted == false).FirstOrDefaultAsync();

            }
            else if(request.WithDepartment==false && request.WithEmployee )
            {
                company = await _context.Companies.Include(i => i.Employees).ThenInclude(i => i.User).Where(i => i.PublicKey == id && i.IsDeleted == false).FirstOrDefaultAsync();

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
                        company.Id,
                        company.PublicKey,
                        company.Name,
                        company.Logo_Url,
                        company.Description,
                       
                        Employees = company.Employees?.Select(e => new
                        {  
                            e.PublicKey,
                            e.User?.Email,
                            e.User?.FirstName,
                            e.User?.LastName,
                           
                           
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

        public async Task<MainResponse> InviteUser(InviteUserRequest request, int userId)
        {
            string companyIdentifier = request.CompanyIdentifier;
            string userIdentifier = request.UserIdentifier;

            companny findedCompany = await _context.Companies.Include(i=>i.Employees).Where(j => j.PublicKey == companyIdentifier).FirstOrDefaultAsync();

            if (findedCompany == null)
            {
                return new MainResponse()
                {
                    Code = 400,
                    Message = "company does not exist",
                    Status = 400,
                    Success = false,
                    Type = "Not found"
                };
            };
            userr findedUser = await _context.Users.Where(i => i.PublicKey == userIdentifier).FirstOrDefaultAsync();

            if(findedUser == null)
            {
                return new MainResponse()
                {
                    Code = 400,
                    Message = "user does not exist",
                    Status = 400,
                    Success = false,
                    Type = "Not found"
                };
            }
            Employee findedEmployee = findedCompany.Employees.Where(i => i.UserId == findedUser.Id).FirstOrDefault();
            if (findedEmployee != null)
            {
                return new MainResponse()
                {
                    Code = 400,
                    Message = "this user is already an employee in your company",
                    Status = 400,
                    Success = false,
                    Type = "already employee"
                };
            }
            if (findedCompany.CreatedBy == findedUser.Id)
            {
                return new MainResponse()
                {
                    Code = 400,
                    Message = "you cant invite yourself",
                    Status = 400,
                    Success = false,
                    Type = "conflict"
                };
            }
            CompanyInviteUser findedInvitation = await _context.CompaniesInviteduser.Where(i => i.UserId == findedUser.Id && i.CompanyId == findedCompany.Id).FirstOrDefaultAsync();
            if (findedInvitation != null)
            {
                return new MainResponse()
                {
                    Code = 400,
                    Message = "you have a request or invitation from/to this company",
                    Status = 400,
                    Success = false,
                    Type = "already requested/invited"
                };
            }
            await _context.CompaniesInviteduser.AddAsync(new CompanyInviteUser()
            {
                CompanyId = findedCompany.Id,
                joinedByUser = false,
                UserId = findedUser.Id
            });
            Count count = await _context.Counts.Where(i => i.Id == 1).FirstOrDefaultAsync();
            Dictionary<String, object> jsonInfo = JsonSerializer.Deserialize<Dictionary<String, object>>(count.JsonInfo);
            if (jsonInfo.ContainsKey($"{findedCompany.PublicKey} >> invitedUsers"))
            {
                jsonInfo[$"{findedCompany.PublicKey} >> invitedUsers"] = ((JsonElement)jsonInfo[$"{findedCompany.PublicKey} >> invitedUsers"]).GetInt32() + 1;
            }
            else
            {
                jsonInfo[$"{findedCompany.PublicKey} >> invitedUsers"] = 1;
            }
            if (jsonInfo.ContainsKey($"{findedUser.PublicKey} >> requestsToJoin"))
            {
                jsonInfo[$"{findedUser.PublicKey} >> requestsToJoin"] = ((JsonElement)jsonInfo[$"{findedUser.PublicKey} >> requestsToJoin"]).GetInt32() + 1;
            }
            else
            {
                jsonInfo[$"{findedUser.PublicKey} >> requestsToJoin"] = 1;
            }
            count.JsonInfo = JsonSerializer.Serialize(jsonInfo);
            await _context.SaveChangesAsync();

            await _context.SaveChangesAsync();

            return new MainResponse()
            {
                Code = 200,
                Message = "invitation is sended",
                Status = 200,
                Success = true,
                Type = "request done",
                data = new
                {
                    companyId = findedCompany.PublicKey,
                    userId=findedUser.PublicKey
                }
            };




        }

        public async Task<MainResponse> GetInvitations(GetInvitationsRequest request,string id)
        {
            companny findedCompany=await _context.Companies.Where(i=>i.PublicKey==id).FirstOrDefaultAsync();
            if (findedCompany == null)
            {
                return new MainResponse()
                {
                    Code = 400,
                    Message = "company does not exist",
                    Status = 400,
                    Success = false,
                    Type = "not found"
                };
            }
            Count count = await _context.Counts.Where(i => i.Id == 1).FirstOrDefaultAsync();
            Dictionary<String, object> jsonInfo = JsonSerializer.Deserialize<Dictionary<string, object>>(count.JsonInfo);
            int totalInvitations = 0;
            if (jsonInfo.ContainsKey($"{id} >> invitedUsers"))
            {
                totalInvitations =  ((JsonElement)jsonInfo[$"{id} >> invitedUsers"]).GetInt32();

            }


            var invitations = await _context.CompaniesInviteduser.Include(j=>j.User).OrderByDescending(i => i.Id).Where(j => j.CompanyId==findedCompany.Id).Skip(request.StartAt).Take(request.Limit).Select(k => new
            {
                idenitifier = k.PublicKey,
                isJoinedByUser=k.joinedByUser,
                user=new
                {
                    identifier=k.User.PublicKey,
                    profile=k.User.ProfileUrl,
                    email=k.User.Email,
                    name=k.User.FirstName+" "+k.User.LastName,

                },
                status=k.status
 
            }).ToListAsync();

            return new MainResponse()
            {
                Code = 200,
                Status = 200,
                Message = "invitations returned successfully",
                Type = "success",
                Success = true,
                data = new
                {
                    startAt = request.StartAt,
                    limit = request.Limit,
                    invitations = invitations,
                    total= totalInvitations
                }
            };

        }

        public async Task<MainResponse> ReplyForInvite(ReplyForInviteRequest request)
        {
            string inviteId = request.InviteId;
            CompanyInviteUser inviteRequest = await _context.CompaniesInviteduser.Where(i => i.PublicKey == inviteId).FirstOrDefaultAsync();
            if (inviteRequest == null)
            {
                return new MainResponse()
                {
                    Code = 400,
                    Message = "no invite request found",
                    Status = 400,
                    Success = false,
                    Type = "Not Found"
                };
            }
            if (inviteRequest.joinedByUser == false)
            {
                return new MainResponse()
                {
                    Code = 400,
                    Message = "you cant reply for your invite, you can only reply for join",
                    Status = 400,
                    Success = false,
                    Type = "your invite"
                };
            }
            if (inviteRequest.status == 2)
            {
                return new MainResponse()
                {
                    Code = 400,
                    Message = "you cant change rejected status, but you can invite the user another time",
                    Status = 400,
                    Success = false,
                    Type = "rejected"
                };
            }
            if (inviteRequest.status == 1)
            {
                return new MainResponse()
                {
                    Code = 400,
                    Message = "you cant change accepted status, but you can invite the user another time",
                    Status = 400,
                    Success = false,
                    Type = "rejected"
                };
            }
            inviteRequest.status = request.ReplyValue;
            if (request.ReplyValue == 1)
            {
                Rule rule = await _context.Rule.Where(i => i.RuleName == "Employee").FirstOrDefaultAsync();
                Employee employee = new Employee()
                {
                    RuleId = rule.Id,
                    UserId = inviteRequest.UserId,
                    CompanyId=inviteRequest.CompanyId
                   
                };
                await _context.Employees.AddAsync(employee);
                await _context.SaveChangesAsync();
                return new MainResponse()
                {
                    Code = 200,
                    Status = 200,
                    Message = "User accepted successfully",
                    Success = true,
                    Type = "success",
                    data = new
                    {
                        employeeId =employee.PublicKey
                }

                };

            };
            await _context.SaveChangesAsync();
            return new MainResponse()
            {
                Code = 200,
                Status = 200,
                Message = "User rejected successfully",
                Success = true,
                Type = "success",


            };
            


        }

        public async Task<MainResponse> SearchForCompany(SearchForCompanyRequest request)
        {
            var companies = await _context.Companies.Where(i => i.Name.Contains(request.Name)).Select(k => new
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
                    companies = companies
                }
            };
        }
        public async Task<MainResponse> SearchForEmployee(SearchForEmployeeInCompanyRequest request)
        {
            companny company = await _context.Companies.Where(k => k.PublicKey == request.CompanyIdentifier).FirstOrDefaultAsync();
            if (company == null)
            {
                return new MainResponse()
                {
                    Code = 400,
                    Status = 400,
                    Message = "company not found",
                    Success = false,
                    Type = "Conflict"

                };
            }
            List<Employee> employees = await _context.Employees.Include(i => i.User).Where(i => i.CompanyId == company.Id).ToListAsync();


            var searchedEmployees=employees.Where(i=>i.User.Email.Contains(request.Email)).Select(k => new
            {
                id=k.Id,
                identifier = k.PublicKey,
                Email = k.User.Email,
                Name = k.User.FirstName + " " + k.User.LastName,
                Logo = k.User.ProfileUrl,
            }).ToList();








            return new MainResponse()
            {
                Code = 200,
                Status = 200,
                Message = "employees returned successfully",
                Type = "success",
                Success = true,
                data = new
                {
                    employees = searchedEmployees
                }
            };
        }
    }
}
