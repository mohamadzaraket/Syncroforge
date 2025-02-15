using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using SyncroForge.Data;
using SyncroForge.Models;
using SyncroForge.Requests.Guest;
using SyncroForge.Responses.Guest.RegisterResponse;
using Otpp = SyncroForge.Models.Otp;
using BCrypt.Net;
using SyncroForge.Responses.Guest.LoginResponse;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using SyncroForge.Responses.Guest.RefreshToken;
namespace SyncroForge.Services.Guest
{
    public class GuestService : IGuestService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        public GuestService(AppDbContext context,IConfiguration configuration) {
        _context= context;
         _configuration= configuration;
        }

        public async Task<LoginResponse> Login(Requests.Guest.LoginRequest request)
        {
            User? user = await _context.Users.Where(i => i.Email == request.Email).FirstOrDefaultAsync();


            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                return new LoginResponse()
                {
                    Code = 401,
                    Status = 400,
                    Message = "Invalid credentials",
                    Success = false,
                    Type = "Login failed"
                };
            };
            String Token = await GenerateTokenString(user);

            String RefreshToken = GenerateRefreshToken();
            await _context.RefreshTokens.AddAsync(new RefreshToken()
            {
                UserId = user.Id,
                Token = RefreshToken
            });
            await _context.SaveChangesAsync();

            return new LoginResponse() {
                Code = 200,
                Status = 200,
                Success = true,
                Message = "login succeed",
                Type = "authorized",
                data = new
                {
                    refreshToken = RefreshToken,
                    accessToken = Token
                }
            };



        }

        public async Task<RegisterResponse> Register(Requests.Guest.RegisterRequest request)
        {
            User user = await _context.Users.Where(i => i.Email == request.Email).FirstOrDefaultAsync();
            if (user != null)
            {
                return new RegisterResponse()
                {
                    Status = 400,
                    Message = "email already exist",
                    Success = false,
                    Type = "email conflict",
                    Code = 409
                };
            }
            Otpp otpSearch = await _context.Otps.Where(i => i.Email == request.Email && i.OtpNumber==request.Otp).OrderByDescending(i => i.CreatedAt).FirstOrDefaultAsync();

            long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            if (otpSearch == null || otpSearch.ExpiryAt < currentTime)

            {

                return new RegisterResponse()

                {

                    Status = 400,

                    Message = "invalid or expired otp",

                    Success = false,

                    Type = "invalid credential",

                    Code = 400

                };

            }

            await _context.Users.AddAsync(new User()
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password)
            });
            await _context.SaveChangesAsync();
            return new RegisterResponse() { Status = 200, Message = "User is registered successfully", Success = true, Type = "register succeed", Code = 201 };


        }
        private async Task<string> GenerateTokenString(User user)
        {

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("publicId",user.PublicKey),
            };



            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.Now.AddMinutes(60);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiry,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private  string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public async Task<RefreshTokenResponse> RefreshToken(RefreshTokenRequest request)
        {
            RefreshToken? token = await _context.RefreshTokens.Include(i=>i.User).Where( i=>i.Token==request.RefreshToken).FirstOrDefaultAsync();
            long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (token == null|| token.ExpiryAt< currentTime)
            {
                  return new RefreshTokenResponse()

                {

                    Status = 400,

                    Message = "invalid or expired token",

                    Success = false,

                    Type = "invalid credential",

                    Code = 400

                };
            }
             Console.WriteLine(token.User);
            String Token = await GenerateTokenString(token.User);
          

            String RefreshToken = GenerateRefreshToken();
            await _context.RefreshTokens.AddAsync(new RefreshToken()
            {
                UserId = token.User.Id,
                Token = RefreshToken
            });
            await _context.SaveChangesAsync();

            return new RefreshTokenResponse()
            {
                Code = 200,
                Status = 200,
                Success = true,
                Message = "new Token generated successfully",
                Type = "authorized",
                data = new
                {
                    refreshToken = RefreshToken,
                    accessToken = Token
                }
            };

        }
    }
}
