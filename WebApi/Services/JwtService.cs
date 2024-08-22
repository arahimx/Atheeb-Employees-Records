using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using RestApi.Interfaces;
namespace RestApi.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtService> _logger;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly string _securityKey;
        private readonly int _tokenExpiryMinutes;

        public JwtService(IConfiguration configuration, ILogger<JwtService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _issuer = _configuration["Jwt:Issuer"];
            _audience = _configuration["Jwt:Audience"];
            _securityKey = _configuration["Jwt:SecurityKey"];
            _tokenExpiryMinutes = int.Parse(_configuration["Jwt:TokenExpiryMinutes"]);
        }

        public string GenerateJwtToken(IDictionary<string, string> userClaims)
        {
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_securityKey));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };
                foreach (var claim in userClaims)
                {
                    claims.Add(new Claim(claim.Key, claim.Value));
                }
                var token = new JwtSecurityToken(
                    issuer: _issuer,
                    audience: _audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(_tokenExpiryMinutes),
                    signingCredentials: credentials
                );
                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                _logger.LogInformation($"Generated JWT token with claims: {string.Join(", ", userClaims.Select(c => $"{c.Key}: {c.Value}"))}");
                return tokenString;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating JWT token.");
                throw new InvalidOperationException("Error generating JWT token.", ex);
            }
        }

        public (bool IsValid, ClaimsPrincipal Principal, string Message) VerifyToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateLifetime = true,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_securityKey)),
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                if (validatedToken.ValidTo < DateTime.UtcNow)
                {
                    _logger.LogWarning("Token expired.");
                    return (false, null, "الرمز المميز منتهي الصلاحية.");
                }
                _logger.LogInformation("Token validated successfully.");
                return (true, principal, "Token is valid.");
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogWarning(ex, "Token validation failed.");
                return (false, null, "Token validation failed: " + ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during token validation.");
                return (false, null, "An unexpected error occurred during token validation.");
            }
        }
    }
}