//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.IdentityModel.Tokens;
//using System;
//using System.IdentityModel.Tokens.Jwt;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace RestApi.Middlewares
//{
//    public class JwtTokenMiddleware
//    {
//        private readonly RequestDelegate _next;
//        private readonly IConfiguration _configuration;

//        public JwtTokenMiddleware(RequestDelegate next, IConfiguration configuration)
//        {
//            _next = next;
//            _configuration = configuration;
//        }

//        public async Task Invoke(HttpContext context)
//        {
//            var BaseUrl = _configuration["BaseUrl"];
//            var path = context.Request.Path.Value.ToLower();

//            if (!string.IsNullOrWhiteSpace(BaseUrl))
//            {
//                BaseUrl = $"{BaseUrl}";
//            }
//            var excludedPaths = new[]
//        {
//            $"{BaseUrl}/api/auth/post_signin",
//            $"{BaseUrl}/api/auth/post_signup",
//            $"{BaseUrl}/api/auth/post_forgetpass",
//            $"{BaseUrl}/api/auth/post_resetpass",
//            $"{BaseUrl}/api/auth/post_verifyotp",
//            $"{BaseUrl}/api/auth/post_sendotp"
//        };
//            if (excludedPaths.Contains(path))
//            {
//                await _next(context);
//                return;
//            }

//            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
//            if (token != null)
//            {
//                try
//                {
//                    var tokenHandler = new JwtSecurityTokenHandler();
//                    var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);
//                    tokenHandler.ValidateToken(token, new TokenValidationParameters
//                    {
//                        ValidateIssuerSigningKey = true,
//                        IssuerSigningKey = new SymmetricSecurityKey(key),
//                        ValidateIssuer = false,
//                        ValidateAudience = false,
//                        ClockSkew = TimeSpan.Zero
//                    }, out _);
//                }
//                catch (Exception)
//                {
//                    context.Response.StatusCode = 401;
//                    await context.Response.WriteAsync("Invalid token");
//                    return;
//                }
//            }
//            else
//            {
//                context.Response.StatusCode = 401;
//                await context.Response.WriteAsync("Token is missing");
//                return;
//            }

//            await _next(context);
//        }
//    }

//    public static class JwtTokenMiddlewareExtensions
//    {
//        public static IServiceCollection AddJwtTokenMiddleware(this IServiceCollection services)
//        {
//            return services.AddTransient<JwtTokenMiddleware>();
//        }
//    }
//}
