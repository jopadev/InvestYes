using BuildingBlocks.Messaging.Consumers;
using FluentValidation;
using InvestYes.Application.Mappings;
using InvestYes.Infrastructure.RabbitMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using InvestYes.Infrastructure.RabbitMQ.Consumers;
using InvestYes.Domain.Services;
using InvestYes.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using InvestYes.BuildingBlocks.Messaging.Abstractions;
using InvestYes.Application.Behaviors;
using MediatR;
using InvestYes.Infrastructure.ExternalApis;
using InvestYes.Infrastructure.ExternalServices.Brapi;
using Microsoft.Extensions.Logging;
using InvestYes.Infrastructure.ExternalApis.Yahoo;
using InvestYes.Infrastructure.Persistence.Repositories;
using InvestYes.Infrastructure.Persistence.Dapper;
using InvestYes.Infrastructure.ExternalApis.MarketData;
using InvestYes.Domain.Interfaces.Repositories;
using InvestYes.Application.Features.Assets.Commands.CreateAsset;
using InvestYes.Application.Interfaces;
using InvestYes.Application.Services;
using InvestYes.Infrastructure.ExternalApis.FundsExplorer;

namespace InvestYes.IoC
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMQ"));

            services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();

            services.AddSingleton<IConnection>(sp =>
            {
                var factory = new ConnectionFactory
                {
                    HostName = "localhost",
                    UserName = "guest",
                    Password = "guest"
                };

                return factory.CreateConnectionAsync().Result;
            });

            services.AddScoped<IEventPublisher, RabbitMqPublisher>();

            services.AddSingleton<IRabbitMqInitializer, RabbitMqInitializer>();

            services.AddHealthChecks()
                .AddCheck<RabbitMqHealthCheck>("rabbitmq")
                .AddNpgSql(
                    configuration.GetConnectionString("DefaultConnection")!);

            services.AddHostedService<AssetCreatedConsumer>();


            return services;
        }

        public static IServiceCollection Register(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(typeof(AutoMapperProfile));

            services.AddValidatorsFromAssemblyContaining<CreateAssetCommandValidator>();

            services.AddScoped<IAssetRepository, AssetRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
            services.AddScoped<IDapperRepository, DapperRepository>();
            services.AddScoped<IUserReadOnlyRepository, UserReadOnlyRepository>();
            services.AddScoped<IAssetReadOnlyRepository, AssetReadOnlyRepository>();

            services.AddMessaging(configuration);

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.Load("InvestYes.Application"));
            });

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            services.AddMemoryCache();

            services.AddScoped<IMarketDataProvider, BrapiMarketDataProvider>();

            services.AddScoped<IMarketDataProvider, YahooFinanceMarketDataProvider>();

            services.AddScoped<IInvestmentAnalysisService, InvestmentAnalysisService>();

            services.Configure<MarketDataOptions>(configuration.GetSection("MarketData"));

            services.AddProviders(configuration);

            services.AddDbContext<InvestYesContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                    npgsql =>
                    {
                        npgsql.MigrationsAssembly(typeof(InvestYesContext).Assembly.FullName);
                        npgsql.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorCodesToAdd: null);
                    });
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }


        public static IServiceCollection AddProviders(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IFiiScoreService, FiiScoreService>();

            services.AddHttpClient<BrapiMarketDataProvider>();

            services.AddHttpClient<YahooFinanceMarketDataProvider>();

            services.AddHttpClient<FundsExplorerCrawlerProvider>(client =>
            {
                client.DefaultRequestHeaders.Clear();

                client.DefaultRequestHeaders.Add("User-Agent","Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/138.0.0.0 Safari/537.36");

                client.DefaultRequestHeaders.Add("Accept","text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");

                client.DefaultRequestHeaders.Add("Accept-Language", "pt-BR,pt;q=0.9");

                client.DefaultRequestHeaders.Add("Referer","https://www.google.com/");

                client.DefaultRequestHeaders.Add("Cache-Control","no-cache");

                client.DefaultRequestHeaders.ConnectionClose = false;
            });

            services.AddScoped<IMarketDataProvider>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<CompositeMarketDataProvider>>();

                return new CompositeMarketDataProvider(
                [
                    sp.GetRequiredService<BrapiMarketDataProvider>(),
                    sp.GetRequiredService<FundsExplorerCrawlerProvider>(),
                    sp.GetRequiredService<YahooFinanceMarketDataProvider>()
                ],
                logger);
            });

            return services;
        }
    }
}
