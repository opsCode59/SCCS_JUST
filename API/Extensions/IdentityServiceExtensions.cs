using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API.Extensions;
    public static class IdentityServiceExtensions
        {
    public static IServiceCollection  AddIdentityServices(this IServiceCollection services,IConfiguration configuration) {


        services.AddIdentityCore<AppUser>(opt =>
        {
            opt.Password.RequireNonAlphanumeric = false;
        })
            .AddRoles<AppRole>()
            .AddRoleManager<RoleManager<AppRole>>()
            .AddEntityFrameworkStores<AppDbContext>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(option =>
        {
            option.TokenValidationParameters = new TokenValidationParameters
                {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["TokenKey"])),
                ValidateAudience = false,
                ValidateIssuer = false,
                };
            option.Events = new JwtBearerEvents
                {
                 OnMessageReceived = context =>
                 {
                     var accessToken = context.Request.Query["access_token"];
                     var path = context.HttpContext.Request.Path;
                     if(! string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                         {
                         context.Token = accessToken;
                         }
                     return Task.CompletedTask;
                 }
                };
        });
        services.AddAuthorization(opt => {
            opt.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
            opt.AddPolicy("RequireClinic", policy => policy.RequireRole("Clinic"));
            opt.AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin"));
        });
        return services;
        }

        }