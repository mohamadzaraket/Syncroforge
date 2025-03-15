using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SyncroForge.Models;
using SyncroForge.Requests.Comany;
using SyncroForge.Requests.Company;
using SyncroForge.Responses;
using SyncroForge.Services.Company;
using System.ComponentModel.Design;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SyncroForge.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }
        [HttpPost]
        public async Task<IActionResult> AddCompany([FromForm]AddCompanyRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                string token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                string publicId = jwtToken.Claims.FirstOrDefault(c => c.Type == "publicId")?.Value;
                string userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int userId =int.Parse(userIdString);
               
                MainResponse response = await _companyService.AddCompany(request, publicId, userId);
                return StatusCode(response.Status, response);

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(400, new
                {
                    status = 400,
                    message = "error while adding company"
                });
            }

        }
        [HttpGet]
        public async Task<IActionResult> GetCompanies([FromQuery] GetCompaniesRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                string token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                string publicId = jwtToken.Claims.FirstOrDefault(c => c.Type == "publicId")?.Value;
                string userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int userId = int.Parse(userIdString);

                MainResponse response = await _companyService.GetCompanies(request, userId, publicId);
                return StatusCode(response.Status, response);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(400, new
                {
                    status = 400,
                    message = "error while adding company"
                });
            }

        }
        [HttpGet]
        public async Task<IActionResult> SearchForCompany([FromQuery] SearchForCompanyRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                MainResponse response = await _companyService.SearchForCompany(request);
                return StatusCode(response.Status, response);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(400, new
                {
                    status = 400,
                    message = "error while search for company"
                });
            }

        }

        [HttpPut]
        public async Task<IActionResult> UpdateCompany([FromForm] UpdateCompanyRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                string token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                string publicId = jwtToken.Claims.FirstOrDefault(c => c.Type == "publicId")?.Value;
                string userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int userId = int.Parse(userIdString);

                MainResponse response = await _companyService.UpdateCompany(request, publicId, userId);
                return StatusCode(response.Status, response);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(400, new
                {
                    status = 400,
                    message = "error while Updating company"
                });
            }

        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetCompany([FromQuery] GetCompanyRequest request ,string id)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            MainResponse response = await _companyService.GetCompany(request ,id);
            return StatusCode(response.Status, response);

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return StatusCode(400, new
            {
                status = 400,
                message = "error while adding company"
            });
        }

    }
        [HttpPost]
        public async Task<IActionResult> InviteUser(InviteUserRequest request)
        {
            try
            {
                string userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int userId = int.Parse(userIdString);
                MainResponse response = await _companyService.InviteUser(request, userId);
                return StatusCode(response.Status, response);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(400, new
                {
                    status = 400,
                    message = "error while inviting user"
                });
            }

        }
        [HttpGet("{companyId}")]
        public async Task<IActionResult> GetInvitations([FromQuery]GetInvitationsRequest request,string companyId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                string userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int userId = int.Parse(userIdString);
                MainResponse response = await _companyService.GetInvitations(request, companyId);
                return StatusCode(response.Status, response);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(400, new
                {
                    status = 400,
                    message = "error while inviting user"
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> ReplyForInvite([FromBody] ReplyForInviteRequest request)
        {
            try
            {
                MainResponse response = await _companyService.ReplyForInvite(request);
                return StatusCode(response.Status, response);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(400, new
                {
                    status = 400,
                    message = "error while replying for invite"
                });
            }
        }

    }
  }