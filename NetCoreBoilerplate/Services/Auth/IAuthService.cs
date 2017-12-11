using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NetCoreBoilerplate.Services.Auth
{
    public interface IAuthService
    {
        string GenerateEncodedToken(string email, List<Claim> claims, bool rememberMe);
        List<Claim> GenerateClaimsIdentity(string email, string id);
    }
}