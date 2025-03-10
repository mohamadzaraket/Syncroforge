using Microsoft.EntityFrameworkCore;
using SyncroForge.Data;
using SyncroForge.Models;
using SyncroForge.Requests.User;
using SyncroForge.Responses;
using System.Text.Json;
using companny = SyncroForge.Models.Company;
using userr = SyncroForge.Models.User;
namespace SyncroForge.Services.User
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _appDbContext;
        public UserService(AppDbContext appDbContext) {
            _appDbContext = appDbContext;
                }

        public async Task<MainResponse> GetInvitations(GetInvitationsRequest request, int userId, string publicUserId)
        {

            
            Count count = await _appDbContext.Counts.Where(i => i.Id == 1).FirstOrDefaultAsync();
            Dictionary<String, object> jsonInfo = JsonSerializer.Deserialize<Dictionary<string, object>>(count.JsonInfo);
            int totalInvitations = 0;
            if (jsonInfo.ContainsKey($"{publicUserId} >> requestsToJoin"))
            {
                totalInvitations = ((JsonElement)jsonInfo[$"{publicUserId} >> requestsToJoin"]).GetInt32();

            }


            var invitations = await _appDbContext.CompaniesInviteduser.Include(j => j.Company).OrderByDescending(i => i.Id).Where(j => j.UserId == userId).Skip(request.StartAt).Take(request.Limit).Select(k => new
            {
                idenitifier = k.PublicKey,
                isJoinedByUser = k.joinedByUser,
                company = new
                {
                    identifier = k.Company.PublicKey,
                    logo = k.Company.Logo_Url,
                    name = k.Company.Name
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
                    total = totalInvitations
                }
            };
        }

        public async Task<MainResponse> GetProfileInfo(int userId)
        {
            userr user = await _appDbContext.Users.Where(i => i.Id == userId).FirstOrDefaultAsync();

            return new MainResponse()
            {
                Code = 200,
                data = new
                {
                    name = $"{user.FirstName} {user.LastName}",
                    email = user.Email,
                    profilePhoto = user.ProfileUrl
                },
                Message="profile returned successfully",
                Status=200,
                Success=true,
                Type="success"
            };
        }

        public async Task<MainResponse> JoinCompany(JoinCompanyRequest request,int userId,string publicUserId)
        {
            companny findedCompany = await _appDbContext.Companies.Include(i=>i.Employees).Where(j => j.PublicKey == request.CompanyIdentifier).FirstOrDefaultAsync();
            if(findedCompany == null)
            {
                return new MainResponse()
                {
                    Code = 400,
                    Message = "company does not exist",
                    Status = 400,
                    Success = false,
                    Type = "no company"
                };
            }
            Employee findedEmployee = findedCompany.Employees.Where(i => i.UserId == userId).FirstOrDefault();
            if (findedEmployee != null)
            {
                return new MainResponse()
                {
                    Code = 400,
                    Message = "you are already an employee in this company",
                    Status = 400,
                    Success = false,
                    Type = "already joined"
                };
            }
            if (findedCompany.CreatedBy == userId)
            {
                return new MainResponse()
                {
                    Code = 400,
                    Message = "you cant join your own company",
                    Status = 400,
                    Success = false,
                    Type = "company creator"
                };
            }
            CompanyInviteUser findedInvitation = await _appDbContext.CompaniesInviteduser.Where(i => i.UserId == userId && i.CompanyId == findedCompany.Id).FirstOrDefaultAsync();
            if(findedInvitation != null)
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

            await _appDbContext.CompaniesInviteduser.AddAsync(new CompanyInviteUser()
            {
                CompanyId = findedCompany.Id,
                joinedByUser = true,
                UserId = userId
            });
            Count count = await _appDbContext.Counts.Where(i => i.Id == 1).FirstOrDefaultAsync();
            Dictionary<String, object> jsonInfo = JsonSerializer.Deserialize<Dictionary<String, object>>(count.JsonInfo);
            if (jsonInfo.ContainsKey($"{publicUserId} >> requestsToJoin"))
            {
                jsonInfo[$"{publicUserId} >> requestsToJoin"] = ((JsonElement)jsonInfo[$"{publicUserId} >> requestsToJoin"]).GetInt32() + 1;
            }
            else
            {
                jsonInfo[$"{publicUserId} >> requestsToJoin"] = 1;
            }
            if (jsonInfo.ContainsKey($"{findedCompany.PublicKey} >> invitedUsers"))
            {
                jsonInfo[$"{findedCompany.PublicKey} >> invitedUsers"] = ((JsonElement)jsonInfo[$"{findedCompany.PublicKey} >> invitedUsers"]).GetInt32() + 1;
            }
            else
            {
                jsonInfo[$"{findedCompany.PublicKey} >> invitedUsers"] = 1;
            }
            count.JsonInfo = JsonSerializer.Serialize(jsonInfo);

            await _appDbContext.SaveChangesAsync();
            await _appDbContext.SaveChangesAsync();
            return new MainResponse()
            {
                Code = 200,
                Message = "request to join company is sended",
                Status = 200,
                Success = true,
                Type = "request done",
                data=new
                {
                    companyId= findedCompany.PublicKey
                }
            };


        }

        public async Task<MainResponse> ReplyForInvite(ReplyForInviteRequest request)
        {
            string inviteId = request.InviteId;
            CompanyInviteUser inviteRequest = await _appDbContext.CompaniesInviteduser.Where(i => i.PublicKey == inviteId).FirstOrDefaultAsync();
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
            if (inviteRequest.joinedByUser == true)
            {
                return new MainResponse()
                {
                    Code = 400,
                    Message = "you cant reply for your join request, you can only reply for invite",
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
                    Message = "you cant change rejected status",
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
                    Message = "you cant change accepted status",
                    Status = 400,
                    Success = false,
                    Type = "rejected"
                };
            }
            inviteRequest.status = request.ReplyValue;
            if (request.ReplyValue == 1)
            {
                Rule rule = await _appDbContext.Rule.Where(i => i.RuleName == "Employee").FirstOrDefaultAsync();
                Employee employee = new Employee()
                {
                    RuleId = rule.Id,
                    UserId = inviteRequest.UserId,
                    CompanyId = inviteRequest.CompanyId

                };
                await _appDbContext.Employees.AddAsync(employee);
                await _appDbContext.SaveChangesAsync();
                return new MainResponse()
                {
                    Code = 200,
                    Status = 200,
                    Message = "User joined successfully",
                    Success = true,
                    Type = "success",
                    data = new
                    {
                        employeeId = employee.PublicKey
                    }

                };

            };
            await _appDbContext.SaveChangesAsync();
            return new MainResponse()
            {
                Code = 200,
                Status = 200,
                Message = "User reject invitation succesfully",
                Success = true,
                Type = "success",


            };



        }
    }
}
