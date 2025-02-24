using Microsoft.EntityFrameworkCore;
using SyncroForge.Data;
using SyncroForge.Models;
using SyncroForge.Responses;

namespace SyncroForge.Services.StatusService
{
    public class StatusService : IStatusService
    {
        private readonly AppDbContext _context;
        public StatusService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<MainResponse> GetStatus()
        {

            List<Status> statuses = await _context.Statuses.ToListAsync();
            return new MainResponse()
            {
                Code = 200,
                data = new
                {
                    statuses = statuses
                },
                Message = "statuses recieved successfully",
                Status = 200,
                Success = true,
                Type = "succcess"
            };
        }
    }
}
