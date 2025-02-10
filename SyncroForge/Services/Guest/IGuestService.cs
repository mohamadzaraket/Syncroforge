using Microsoft.AspNetCore.Identity.Data;
using SyncroForge.Data;
using SyncroForge.Requests.Guest;
using SyncroForge.Responses.Guest.RegisterResponse;

namespace SyncroForge.Services.Guest
{
    public interface IGuestService
    {


        public Task<RegisterResponse> Register(Requests.Guest.RegisterRequest request);

    }
}
