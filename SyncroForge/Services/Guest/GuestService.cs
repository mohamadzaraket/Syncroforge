using Microsoft.AspNetCore.Identity.Data;
using SyncroForge.Data;
using SyncroForge.Requests.Guest;
using SyncroForge.Responses.Guest.RegisterResponse;

namespace SyncroForge.Services.Guest
{
    public class GuestService : IGuestService
    {
        private readonly AppDbContext _context;
        public GuestService(AppDbContext context) {
        _context= context;
        }
          
        public async Task<RegisterResponse> Register(Requests.Guest.RegisterRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
