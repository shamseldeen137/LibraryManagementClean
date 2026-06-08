using System.Text;
using LibraryManagement.Application.Auth;
using LibraryManagement.Application.Common;
using LibraryManagement.Application.ExternalCatalog;
using LibraryManagement.Infrastructure.Auth;
using LibraryManagement.Infrastructure.Caching;
using LibraryManagement.Infrastructure.ExternalCatalog;
using LibraryManagement.Infrastructure.Messaging;
using LibraryManagement.Infrastructure.Mongo;
using LibraryManagement.Infrastructure.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace LibraryManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        MongoConventions.Configure();

        services.Configure<MongoOptions>(configuration.GetSection("Mongo"));
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMq"));
        services.Configure<KafkaOptions>(configuration.GetSection("Kafka"));
        services.Configure<MessagingOptions>(configuration.GetSection("Messaging"));
        services.Configure<ExternalCatalogOptions>(configuration.GetSection("ExternalCatalog"));

        services.AddSingleton<MongoDbContext>();
        services.AddScoped(typeof(IRepository<>), typeof(MongoRepository<>));
        services.AddScoped<IUserRepository, MongoUserRepository>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<ICacheService, RedisCacheService>();
        services.AddSingleton<IMessagePublisherStrategy, RabbitMqMessagePublisherStrategy>();
        services.AddSingleton<IMessagePublisherStrategy, KafkaMessagePublisherStrategy>();
        services.AddSingleton<IEventBus, MessagePublisherEventBus>();
        services.AddHttpClient<IExternalBookCatalog, OpenLibraryBookCatalogAdapter>((_, client) =>
        {
            var options = configuration.GetSection("ExternalCatalog").Get<ExternalCatalogOptions>() ?? new ExternalCatalogOptions();
            client.BaseAddress = new Uri(options.BaseUrl.TrimEnd('/'));
        });

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis") ?? "localhost:6379";
            options.InstanceName = "library:";
        });

        var jwt = configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey)),
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

        services.AddAuthorization();
        return services;
    }
}
