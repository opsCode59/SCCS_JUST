﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API.Extensions;
    public static class IdentityServiceExtensions
        {
    public static IServiceCollection  AddIdentityServices(this IServiceCollection services,IConfiguration configuration) {

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(option =>
        {
            option.TokenValidationParameters = new TokenValidationParameters
                {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["TokenKey"])),
                ValidateAudience = false,
                ValidateIssuer = false,
                };
        });
        return services;
        }

        }