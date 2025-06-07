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
using Azure.Core;
using MailKit.Net.Smtp;
using System.Net.Mail;
using MimeKit;

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

            SendEmail("Register Otp", $"your email verification code is:{otp.ToString()}",request.Email);


                return new RegisterResponse()
                {
                    Status = 200,
                    Message = "email sended successfully",
                    Success = true,
                    Type = "succeed",
                    Code = 200
                };






        }
        /*public async System.Threading.Tasks.Task SendEmail(string title, string subject, string otp, string toEmail)
        {
            try
            {
                var requestData = new
                {
                    from = new
                    {
                        email = _configuration["mailSender:fromEmail"],
                        name = _configuration["mailSender:name"]
                    },
                    to = new List<object>
            {
                new { email = toEmail }
            },
                    subject = subject,
                    template_id = _configuration["mailSender:templateId"],
                    personalization = new List<object>
            {
                new
                {
                    email = toEmail,
                    data = new
                    {
                        otp = otp,
                        title = title
                    }
                }
            }
                };

                string jsonString = JsonSerializer.Serialize(requestData, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear(); // clear old headers
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _configuration["mailSender:accessToken"]);
                Console.WriteLine($"TOKEN USED: {_configuration["mailSender:accessToken"]}"); // debug
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await _httpClient.PostAsync(_configuration["mailSender:url"], content);

                string responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to send email. Status: {response.StatusCode}, Body: {responseBody}");
                    // Optionally: throw new Exception($"MailerSend failed: {response.StatusCode}");
                }
                else
                {
                    Console.WriteLine("Email sent successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in SendEmail: {ex.Message}");
                // Optionally rethrow or log in file
            }
        }*/
        public async System.Threading.Tasks.Task SendEmail(string title, string value, string toEmail)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Syncroforge", "Syncroforge@outlook.com"));

                message.To.Add(new MailboxAddress(toEmail, toEmail));
                message.Subject = title;
                var body = value; 
                message.Body = new TextPart("plain")
                {
                    Text = body

                };
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {

                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    client.Connect(_configuration["smtp:server"], int.Parse(_configuration["smtp:port"]), false);


                    client.Authenticate(_configuration["smtp:email-auth"], _configuration["smtp:email-apppassword"]);

                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in SendEmail: {ex.Message}");
                // Optionally rethrow or log in file
            }
        }




    }
}

