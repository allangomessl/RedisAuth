using System;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Authorization;
using AuthBearer.Models;
using AuthBearer.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace AuthBearer.Controllers
{
    [Route("api/accounts")]
    public class AccountController : Controller
    {
        private readonly TokenConfiguration _tokenOptions;
        private readonly ITokenCache _tokens;
        public AccountController(TokenConfiguration tokenOptions, ITokenCache tokens)
        {
            _tokenOptions = tokenOptions;
            _tokens = tokens;
        }

        private string Token(string userName, DateTime expires)
        {
            var handler = new JwtSecurityTokenHandler();

            var identity = new ClaimsIdentity(new GenericIdentity(userName, "TokenAuth"), new[] { new Claim("UserId", "1", ClaimValueTypes.Integer), new Claim(ClaimTypes.Role, "Admin") });

            var tokenDescriptor = new SecurityTokenDescriptor 
            {
                Issuer = _tokenOptions.Inssue,
                Audience = _tokenOptions.Audience,
                SigningCredentials = _tokenOptions.Credentials,
                Subject =  identity,
                Expires = expires
            };

            var securityToken = handler.CreateToken(tokenDescriptor);

            // Escreve o token de segurança
            var token = handler.WriteToken(securityToken);
            return token;
        }



        [AllowAnonymous]
        [HttpPost("login")]
        public dynamic Login([FromBody]User user)
        {
            var expires = DateTime.UtcNow.AddMinutes(2);
            var token = Token(user.UserName, expires);
            var tokenId = _tokens.Add(token);
            return new { authenticated = true, entityId = 1, tokenId = tokenId, tokenExpires = expires };
        }

        [Authorize]
        [HttpGet]
        public string Get() => "Ok Authorized";


        [Authorize]
        [HttpPost("logout")]
        public string Logout()
        {
            var tokenId = HttpContext.Request.Headers["Authorization"].ToString();
            _tokens.Remove(tokenId);
            return "logout";
        }
    }
}
