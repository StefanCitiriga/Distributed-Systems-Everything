using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace DistSysAcwServer.Auth
{
    /// <summary>
    /// Authenticates clients by API Key
    /// </summary>
    public class CustomAuthenticationHandler
        : AuthenticationHandler<AuthenticationSchemeOptions>, IAuthenticationHandler
    {
        private Models.UserContext DbContext { get; set; }
        private IHttpContextAccessor HttpContextAccessor { get; set; }

        public CustomAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            Models.UserContext dbContext,
            IHttpContextAccessor httpContextAccessor)
            : base(options, logger, encoder, clock) 
        {
            DbContext = dbContext;
            HttpContextAccessor = httpContextAccessor;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            #region Task5
            // TODO:  Find if a header ‘ApiKey’ exists, and if it does, check the database to determine if the given API Key is valid
            //        Then create the correct Claims, add these to a ClaimsIdentity, create a ClaimsPrincipal from the identity 
            //        Then use the Principal to generate a new AuthenticationTicket to return a Success AuthenticateResult

            if (!Request.Headers.TryGetValue("ApiKey", out var apiKeyHeaderValues))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            var apiKey = apiKeyHeaderValues.FirstOrDefault();

            if (apiKey == null)
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            var user = DbContext.Users.FirstOrDefault(u => u.ApiKey == apiKey);

            if (user == null)
            {
                return Task.FromResult(AuthenticateResult.Fail("Unauthorized. Check ApiKey in Header is correct."));
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, "ApiKey");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));

            #endregion

            //return Task.FromResult(AuthenticateResult.Fail("Not Authenticated"));
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            byte[] messagebytes = Encoding.ASCII.GetBytes("Task 5 Incomplete");
            Context.Response.StatusCode = 401;
            Context.Response.ContentType = "application/json";
            await Context.Response.Body.WriteAsync(messagebytes, 0, messagebytes.Length);
            await HttpContextAccessor.HttpContext.Response.CompleteAsync();
        }
    }
}