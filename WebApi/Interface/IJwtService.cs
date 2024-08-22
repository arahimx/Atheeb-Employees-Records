using System.Security.Claims;

namespace RestApi.Interfaces
{
    public interface IJwtService
    {
        string GenerateJwtToken(IDictionary<string, string> userClaims);
        (bool IsValid, ClaimsPrincipal Principal, string Message) VerifyToken(string token);
    }
}
