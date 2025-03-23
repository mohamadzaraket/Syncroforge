
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SyncroForge.Data;
using SyncroForge.Hubs;
using SyncroForge.Mapper;
using SyncroForge.Services;
using SyncroForge.Services.AttachmentsService;
using SyncroForge.Services.Company;
using SyncroForge.Services.EmployeeService;
using SyncroForge.Services.Guest;
using SyncroForge.Services.Otp;
using SyncroForge.Services.StatusService;
using SyncroForge.Services.TaskService;
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
            builder.Services.AddSwaggerGen(c =>
            {
                c.CustomSchemaIds(type => type.FullName); // Use fully qualified name to avoid conflicts
            });
            Console.WriteLine("env:"+Env.currentEnvironment);
            builder.Configuration
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            builder.Configuration.AddJsonFile($"appsettings.{Env.currentEnvironment}.json", optional: true, reloadOnChange: true);
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy =>
                    {
                        policy.WithOrigins(builder.Configuration.GetSection("Frontend:Url").Value)
                              .AllowAnyMethod()
                              .AllowAnyHeader()
                              .AllowCredentials(); // ? Required for WebSockets
                    });
            });
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
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddSignalR();
            builder.Services.AddHttpClient();
            builder.Services.AddTransient<IGuestService, GuestService>();
            builder.Services.AddTransient<IOtpService, OtpService>();
            builder.Services.AddSingleton<MinioService>();
            builder.Services.AddTransient<IDepartmentService, DepartmentService>();
            builder.Services.AddTransient<ICompanyService, CompanyService>();
            builder.Services.AddTransient<IUserService,UserService>();
            builder.Services.AddTransient<IDepartmentEmployeeService, departmentEmployeeService>();
            builder.Services.AddTransient<IStatusService, StatusService>();
            builder.Services.AddTransient<IEmployeeService, EmployeeService>();
            builder.Services.AddTransient<ITaskService, TaskService>();
            builder.Services.AddTransient<IAttendanceService, AttendanceService>();
            builder.Services.AddTransient<IAttachmentService, AttachmentService>();

            var app = builder.Build();
            app.Urls.Add("http://0.0.0.0:5000");
            app.Urls.Add("https://0.0.0.0:7220"); // Allow HTTPS on port 7220
            app.UseCors("AllowAll"); // ? Apply CORS Policy


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
          //  app.UseRouting();

            app.MapControllers();
            app.MapHub<TaskHub>("/taskHub");

            app.Run();
        }
    }
}
