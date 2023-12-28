using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Net8CoreApiBoilerplate.Api.Infrastructure.Helpers
{
    public class AuthenticationHelper
    {
        // If current setup doesn't work, I found solution here:
        // Main one that contains almost all of the solution to all of the problems there are with identity: https://github.com/dotnet/aspnetcore/issues/2193
        // Can try here as well: https://wildermuth.com/2018/04/10/Using-JwtBearer-Authentication-in-an-API-only-ASP-NET-Core-Project
        // and also here: https://stackoverflow.com/questions/52038054/web-api-core-returns-404-when-adding-authorize-attribute
        public static void ConfigureService(IServiceCollection service, string issuer, string audience, string secretKey)
        {
            service.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                var secretkey = Encoding.UTF8.GetBytes(secretKey);

                var validationParameters = new TokenValidationParameters
                {
                    // Clock skew is needed becuase of this: https://stackoverflow.com/questions/43045035/jwt-token-authentication-expired-tokens-still-working-net-core-web-api
                    ClockSkew = TimeSpan.Zero, // default: 5 min
                    RequireSignedTokens = true,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secretkey),

                    RequireExpirationTime = true,
                    ValidateLifetime = true,

                    ValidateAudience = true, //default : false
                    ValidAudience = audience,

                    ValidateIssuer = true, //default : false
                    ValidIssuer = issuer,

                    // If you need to have additional layer of keys, pass one into function and use it here. You will have to use it everywhere else though.
                    // TokenDecryptionKey = new SymmetricSecurityKey(encryptionkey)
                };

                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = validationParameters;
            });


            service.AddAuthorization();


            #region In case of Identity server usage, code below will replace one on top
            /*
            //services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
            //    .AddIdentityServerAuthentication(options =>
            //    {
            //        options.Authority = Settings.AuthBaseUrl;
            //        options.ApiName = Settings.AuthApiName;
            //        options.RequireHttpsMetadata = false;
            //    });

            service
                // .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)

                // .AddAuthentication(o => o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme) //Originally was like this

                .AddAuthentication(cfg =>
                {
                    //cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    //cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

                    cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    cfg.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(o =>
                {
                    //o.RequireHttpsMetadata = false;
                    //o.SaveToken = false;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,

                        ValidIssuer = Issuer,
                        ValidAudience = Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key))
                    };
                });

            service.AddAuthorization();
            */
        #endregion
        }

        public static void ConfigureServiceSimple(IServiceCollection services, string issuer, string audience, string secretKey)
        {
            //var keyByteArray = Encoding.ASCII.GetBytes(SecretKey);
            //var signinKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(keyByteArray);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
                // IssuerSigningKey = signinKey,
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = true,
                ValidateLifetime = true,
                ValidAudience = audience
            };

            services.AddSingleton(tokenValidationParameters);
            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.SaveToken = true;
                    x.TokenValidationParameters = tokenValidationParameters;
                });

            services.AddAuthorization();
        }
    }
}
