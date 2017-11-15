using Abp.DoNetCore.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace Abp.DoNetCore
{
    public static class AbpApplicationBuilderExtensions
    {
        //private const string SecretKey = "needtogetthisfromenvironment";
        //private readonly static SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));

        public static void UserAbp(this IApplicationBuilder app, IConfigurationRoot configuration)
        {
            // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
            //var jwtAppSettingOptions = configuration.GetSection(nameof(JwtIssuerOptions));
            //var tokenValidationParameters = new TokenValidationParameters
            //{
            //    ValidateIssuer = true,
            //    ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

            //    ValidateAudience = true,
            //    ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

            //    ValidateIssuerSigningKey = true,
            //    IssuerSigningKey = _signingKey,

            //    RequireExpirationTime = true,
            //    ValidateLifetime = true,

            //    ClockSkew = TimeSpan.Zero
            //};
            app.UseAuthentication();
            //app.UseJwtBearerAuthentication(new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerOptions
            //{

            //    TokenValidationParameters = tokenValidationParameters
            //});
            //app.UseJwtBearerAuthentication(new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerOptions { TokenValidationParameters = tokenValidationParameters });
            //InitializeAbp(app);
        }
        private static void InitializeAbp(IApplicationBuilder app)
        {
            var abpBootstrapper = app.ApplicationServices.GetRequiredService<AbpBootstrapper>();
            abpBootstrapper.Initialize();
        }
    }
}
