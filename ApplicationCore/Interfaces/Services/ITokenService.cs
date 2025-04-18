using System.Security.Claims;
using ApplicationCore.Entities;

namespace ApplicationCore.Interfaces.Services;

public interface ITokenService
{
    string GenerateJwtToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    Guid? ValidateJwtToken(string token);
}