using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthBearer.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace AuthBearer
{
    public class AuthorizationEvents : IJwtBearerEvents
    {
        private readonly ITokenCache _tokenCache;

        public AuthorizationEvents(ITokenCache tokenCache)
        {
            _tokenCache = tokenCache;
        }

        public Task AuthenticationFailed(AuthenticationFailedContext context)
        {
            return Task.FromResult(0);
        }

        public Task Challenge(JwtBearerChallengeContext context)
        {
            // var authId = context.HttpContext.Request.Headers["AuthorizationId"].ToString();
            // if (_tokenCache.Get(authId) == "")
            //     throw new Exception("User not authenticate");
            return Task.FromResult(0);
        }

        public Task MessageReceived(MessageReceivedContext context)
        {    
            var authId = context.HttpContext.Request.Headers["Authorization"].ToString();
            context.Token = _tokenCache.Get(authId);
            return Task.FromResult(0);
        }

        public Task TokenValidated(TokenValidatedContext context)
        {
            return Task.FromResult(0);
        }
    }
}