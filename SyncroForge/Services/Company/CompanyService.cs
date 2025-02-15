using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SyncroForge.Data;
using SyncroForge.Models;
using SyncroForge.Requests.Company;
using SyncroForge.Responses;
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
    }
}
