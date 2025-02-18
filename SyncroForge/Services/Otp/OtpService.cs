using SyncroForge.Data;
using SyncroForge.Requests.Otp;
using SyncroForge.Responses.Otp.RegisterResponse;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System;
using SyncroForge.Models;
using Microsoft.EntityFrameworkCore;
using Otpp = SyncroForge.Models.Otp;
using userr = SyncroForge.Models.User;

namespace SyncroForge.Services.Otp
{
    public class OtpService : IOtpService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        public OtpService(AppDbContext context, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<RegisterResponse> Register(RegisterRequest request)
        {
            userr user =await _context.Users.Where(i=>i.Email==request.Email).FirstOrDefaultAsync();
            if (user != null) {
                return new RegisterResponse()
                {
                    Status = 400,
                    Message = "email already exist",
                    Success = false,
                    Type = "email conflict",
                    Code = 409
                };
            }
            long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            Random random = new Random();
            int otp = random.Next(100000, 999999); ;
            await _context.Otps.AddAsync(new Otpp { Email = request.Email, OtpNumber = otp,type="Register" });
            await _context.SaveChangesAsync();

            SendEmail("Register Otp", "Register Otp", otp.ToString("D6"), request.Email);


                return new RegisterResponse()
                {
                    Status = 200,
                    Message = "email sended successfully",
                    Success = true,
                    Type = "succeed",
                    Code = 200
                };






        }
        public async void SendEmail(string Title, string Subject, string Otp, string toEmail)
        {

            var requestData = new
            {
                from = new
                {
                    email = _configuration.GetSection("mailSender:fromEmail").Value,
                    name = _configuration.GetSection("mailSender:name").Value,

                },
                to = new List<object> {
                    new
                    {
                        email = toEmail
                    }
                },
                subject = Subject,
                template_id = _configuration.GetSection("mailSender:templateId").Value,
                personalization = new List<object>
                {
                    new
                    {
                        email = toEmail,
                        data = new
                        {
                            otp = Otp,
                            title = Title
                        }
                    }
                }


            };
            string jsonString = JsonSerializer.Serialize(requestData, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _configuration.GetSection("mailSender:accessToken").Value);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await _httpClient.PostAsync(_configuration.GetSection("mailSender:url").Value, content);

        }



    }
}

