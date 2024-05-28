using API.Data;
using API.Helpers;
using API.Repository;
using API.SignalR;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;
    public static class ApplicationServiceExtensions
    {
    public static IServiceCollection AddApplicationService( this IServiceCollection services, IConfiguration configuration )
        {
        services.AddDbContextPool<AppDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("ConnectionString"));

        });
        services.AddCors();
        services.AddScoped<ITokenRepository, TokenRepository>();
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
        services.AddScoped<IPhotoRepository, PhotoRepository>();
        services.AddScoped<LogUserActivity>();
        services.AddScoped<IFeedbackRepository, FeedbackRepository>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IScheduleRepository, ScheduleRepository>();
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddSignalR();
        services.AddSingleton<PresenceTracker>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
        }


    }