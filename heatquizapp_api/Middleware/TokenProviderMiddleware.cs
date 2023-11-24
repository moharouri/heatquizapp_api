using HeatQuizAPI.Database;
using HeatQuizAPI.Extensions;
using HeatQuizAPI.Models.BaseModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Xml;

namespace heatquizapp_api.Middleware
{
    public class TokenProviderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ApplicationDbContext _applicationDbContext;

        public TokenProviderMiddleware(
                RequestDelegate next,
                ApplicationDbContext applicationDbContext
            )
        {
            _next = next;
            _applicationDbContext = applicationDbContext;   
        }

        public Task Invoke(HttpContext context, UserManager<User> userManager)
        {
            if (context.Request.Method.Equals("POST") && context.Request.Path.Equals(TokenProviderOptions.Path, StringComparison.Ordinal))
                return GenerateToken(context, userManager);

            return _next(context);
        }

        private async Task GenerateToken(HttpContext context, UserManager<User> userManager)
        {
            var username = "";
            var password = "";
            int? datapoolId = 0;

            //Try reading username, password, datapool Id
            if (context.Request.HasJsonContentType())
            {
                var request = await System.Text.Json.JsonSerializer.DeserializeAsync<LoginRequest>(context.Request.Body);

                username = request?.username;
                password = request?.password;
                datapoolId = request?.datapoolId;

                //Get user
                var user = await userManager.FindByNameAsync(username);

                if (user == null)
                {
                    //Return failed to read login data
                    await HandleFailedLogin(context, "User not found");
                    return;
                }

                //get roles
                var userRoles = await userManager.GetRolesAsync(user);

                //Check if user is admin
                var isUserAdmin = userRoles.Any(r => r.ToLower() == "admin".ToLower());

                //Check password is correct
                var isPasswordValid = await userManager.CheckPasswordAsync(user, password);

                if (!isPasswordValid)
                {
                    //Return failed to read login data
                    await HandleFailedLogin(context, "Incorrect password");
                    return;
                }

                //check datapool exists
                var datapool = await _applicationDbContext.DataPools
                    .Include(dp => dp.PoolAccesses)
                    .FirstOrDefaultAsync(dp => dp.Id == datapoolId);

                if (datapool == null)
                {
                    //Return failed to find datapool
                    await HandleFailedLogin(context, "Datapool does not exist");
                    return;
                }

                //check user has access to datapool
                var userHasAccess = datapool.PoolAccesses.Any(a => a.UserId == user.Id);

                //If user is admin -- no datapool access filtering
                if (!userHasAccess && !isUserAdmin)
                {
                    //Return user have no access to this datapool
                    await HandleFailedLogin(context, "User has no access to this datapool");
                    return;
                }

                //generate token
                //add claims
                List<Claim> claims = new List<Claim>
                    {
                        new Claim(JwtRegisteredClaimNames.Email, user.Email),
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
                    };

                //add roles claims to token to validate user role when accessing restricted controller routes
                foreach (var x in userRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, x));
                }


                //create token
                var _options = new TokenProviderOptions();

                var jwt = new JwtSecurityToken(
                    issuer: _options.Issuer,
                    audience: _options.Audience,
                    claims: claims,
                    notBefore: DateTime.UtcNow,
                    expires: DateTime.UtcNow.Add(_options.Expiration),
                    signingCredentials: _options.SigningCredentials);

                var encodedJwt = (new JwtSecurityTokenHandler()).WriteToken(jwt);

                var response = new
                {
                    access_token = encodedJwt,
                    expires_in = (int)_options.Expiration.TotalSeconds,
                    username = user.Name,
                    roles = userRoles
                };

                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Newtonsoft.Json.Formatting.Indented }));
                return;
            }
            else
            {
                //Return failed to read login data
                await HandleFailedLogin(context, "Failed to load login data");
                return;
            }
        }

        private async Task HandleFailedLogin(HttpContext context, string message)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync(message);
            return;
        }

        class LoginRequest
        {
            public string? username { get; set; }
            public string? password { get; set; }

            public int? datapoolId { get;set; }
        }

        public class TokenProviderOptions
        {
            public static string secretKey = "mysupersecret_secretkey!12345678";

            public static string Path { get; set; } = "/api/Account/Login";

            public string Issuer { get; set; } = "HQAPPWSA";

            public string Audience { get; set; } = "HeatQuizApplicationWSA";

            public TimeSpan Expiration { get; set; } = TimeSpan.FromDays(30);

            public SigningCredentials SigningCredentials { get; set; }
            = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),SecurityAlgorithms.HmacSha256);
        }
    }
}
