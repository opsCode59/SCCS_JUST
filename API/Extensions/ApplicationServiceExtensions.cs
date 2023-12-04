using API.Data;
using API.Repository;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;
    public static class ApplicationServiceExtensions
        {
        public static IServiceCollection AddApplicationService(this IServiceCollection services, IConfiguration configuration) {
        services.AddDbContextPool<AppDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("ConnectionString"));
        });
        services.AddCors();
        services.AddScoped<ITokenRepository, TokenRepository>();
        return services;
        }


        }