using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Pillepalle1.StatefulApi.WebApi;

public static class _DependencyInjection
{
    public static WebApplicationBuilder ConfigureWebApi(this WebApplicationBuilder builder)
    {
        builder.ConfigurePolicies();
        builder.ConfigureJwt();

        return builder;
    }

    private static WebApplicationBuilder ConfigurePolicies(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorizationBuilder()
            .AddPolicy("can-view-books", policy => policy.RequireClaim("can-view-books"))
            .AddPolicy("can-modify-books", policy => policy.RequireClaim("can-modify-books"));

        return builder;
    }

    private static WebApplicationBuilder ConfigureJwt(this WebApplicationBuilder builder)
    {
        var config = builder.Configuration;
        var env = builder.Environment;
        
        // Fetch parameters from configuration
        var jwt = new JwtParameters();
        config.GetSection(JwtParameters.SectionName).Bind(jwt);
        
        // Configure token validation
        var tokenValidationParameters = new TokenValidationParameters()
        {
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = jwt.IssuerSigningKey()
        };

        if (env.IsDevelopment())
        {
            tokenValidationParameters.ValidateIssuer = false;
            tokenValidationParameters.ValidateAudience = false;
            tokenValidationParameters.ValidateLifetime = false;
            tokenValidationParameters.ValidateIssuerSigningKey = false;
        }
        
        // Add Authentication
        builder.Services.AddAuthentication(o =>
        {
            o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            o.TokenValidationParameters = tokenValidationParameters;
        });

        return builder;
    }
}