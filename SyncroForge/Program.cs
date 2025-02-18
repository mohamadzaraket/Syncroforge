
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SyncroForge.Data;
using SyncroForge.Services;
using SyncroForge.Services.Company;
using SyncroForge.Services.Guest;
using SyncroForge.Services.Otp;
using SyncroForge.Services.User;
using System.Text;

namespace SyncroForge
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            Console.WriteLine("env:"+Env.currentEnvironment);
            builder.Configuration
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            builder.Configuration.AddJsonFile($"appsettings.{Env.currentEnvironment}.json", optional: true, reloadOnChange: true);
            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddAuthorization();
            builder.Services.AddAuthentication(options =>


            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;


            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidateActor = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    RequireExpirationTime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration.GetSection("Jwt:Issuer").Value,
                    ValidAudience = builder.Configuration.GetSection("Jwt:Audience").Value,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Jwt:Key").Value))


                };
            });
            builder.Services.AddHttpClient();
            builder.Services.AddTransient<IGuestService, GuestService>();
            builder.Services.AddTransient<IOtpService, OtpService>();
            builder.Services.AddSingleton<MinioService>();
            builder.Services.AddTransient<IDepartmentService, DepartmentService>();
            builder.Services.AddTransient<ICompanyService, CompanyService>();
            builder.Services.AddTransient<IUserService,UserService>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
