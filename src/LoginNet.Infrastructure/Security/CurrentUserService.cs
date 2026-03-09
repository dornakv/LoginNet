using System.Security.Claims;
using LoginNet.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace LoginNet.Infrastructure.Security
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? UserId
        {
            get
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                return int.TryParse(userIdClaim, out var userId) ? userId : null;
            }
        }
    }
}
