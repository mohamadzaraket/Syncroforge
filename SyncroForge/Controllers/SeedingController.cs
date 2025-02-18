using Microsoft.AspNetCore.Mvc;
using SyncroForge.Data;
using SyncroForge.Models;
using SyncroForge.Requests.Company;

namespace SyncroForge.Controllers
{
    [Route("api/[action]")]
    [ApiController]
    public class SeedingController : ControllerBase
    {



        private readonly AppDbContext _context;
        public SeedingController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]

        public async Task<IActionResult> Seed()
        {
            try
            {
                await _context.Statuses.AddRangeAsync(
    new Status { Name = "Done", BackgroundColor = "green", Color = "white" },
    new Status {  Name = "In Progress", BackgroundColor = "blue", Color = "white" },
    new Status {  Name = "Pending", BackgroundColor = "orange", Color = "black" },
    new Status { Name = "On Hold", BackgroundColor = "gray", Color = "white" },
    new Status {  Name = "Cancelled", BackgroundColor = "red", Color = "white" },
    new Status { Name = "Under Review", BackgroundColor = "purple", Color = "white" },
    new Status { Name = "Approved", BackgroundColor = "teal", Color = "white" },
    new Status {  Name = "Rejected", BackgroundColor = "darkred", Color = "white" },
    new Status {  Name = "Waiting for Approval", BackgroundColor = "gold", Color = "black" }
);


                await _context.Rule.AddRangeAsync(
                     new Rule {  RuleName = "Admin" },
                     new Rule {  RuleName = "Employee" }
                    );

                await _context.SaveChangesAsync();


                return StatusCode(200, new
                {
                    status = 200,
                    message = "seeding done successfully"
                });


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(400, new
                {
                    status = 400,
                    message = "error while seeding"
                });
            }

        }


    }
}
