using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace AuthBearer.Authorization
{
    public class TokenConfiguration
    {
        public string Audience = "SampleAudience";
        public string Inssue = "SampleInssue";

        public SecurityKey Key { get; set; }
        public SigningCredentials Credentials;
    }

    public static class AuthorizationConfig
    {

        public static SecurityKey GetSecurityKey(string rootPath)
        {
            var cert = new X509Certificate2(Path.Combine(rootPath, "certificate.pfx"), "1q2w!Q@W", X509KeyStorageFlags.Exportable);
            var rsaParameters = cert.GetRSAPrivateKey().ExportParameters(true);
            return new RsaSecurityKey(rsaParameters);
        }

        public static IServiceCollection AddAuthentication(this IServiceCollection services, string rootPath)
        {
            var tokenOptions = new TokenConfiguration
            {
                Key = GetSecurityKey(rootPath),
            };
            tokenOptions.Credentials = new SigningCredentials(tokenOptions.Key, SecurityAlgorithms.RsaSha256Signature);
            services.AddSingleton(tokenOptions);
            services.AddSingleton<ITokenCache, TokenCache>();
            return services;
        }
        public static IApplicationBuilder UseAuthentication(this IApplicationBuilder app, TokenConfiguration tokenConfig, ITokenCache tokens)
        {
            var options = new JwtBearerOptions
            {
                TokenValidationParameters = new TokenValidationParameters 
                {
                    IssuerSigningKey = tokenConfig.Key,
                    ValidIssuer = tokenConfig.Inssue,
                    ValidAudience = tokenConfig.Audience,
                    ValidateLifetime = true
                }
                
            };
            options.SaveToken = false;
            options.AutomaticAuthenticate = true;
            options.AutomaticChallenge = true;

            options.Events = new AuthorizationEvents(tokens);
            //SaveSigninToken = true;
            //options.TokenValidationParameters.TokenReplayCache = new TokenCache();
            //options.TokenValidationParameters.TokenReplayCache.

            app.UseJwtBearerAuthentication(options);
            return app;
        }
    }

    public interface ITokenCache
    {
        string Add(string token);
        bool Remove(string id);
        string Get(string id);
    }

    public class TokenCache : ITokenCache
    {
        private readonly IDictionary<string, string> _tokens;

        public TokenCache()
        {
            _tokens = new Dictionary<string, string>();
        }

        public string Add(string token)
        {
            var id = Guid.NewGuid().ToString();
            _tokens.Add(id, token);
            return id;
        }

        public bool Remove(string id)
        {
            return _tokens.Remove(id);
        }

        public string Get(string id)
        {
            string token;
            _tokens.TryGetValue(id, out token);
            return token;
        }
        
    }
}