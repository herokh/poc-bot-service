using Hero.Chatbot.Repository;
using Hero.Chatbot.Repository.Contracts;
using Hero.Chatbot.Service;
using Hero.Chatbot.Service.Contracts;
using Hero.Shared.DbContext;
using Hero.Shared.Repository;
using Hero.Shared.UnitOfWork;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data.Common;

namespace Hero.Chatbot.Web
{
    public static class ServiceConfigExtensions
    {
        public static void RegisterInjections(this IServiceCollection services, IConfiguration configuration)
        {
            RegisterHttpClientDependencies(services, configuration);
            RegisterDatabaseDependencies(services);
            RegisterDomainDependencies(services);
        }

        private static void RegisterHttpClientDependencies(IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<IHeroChatbotCMSApiClient, HeroChatbotCMSApiClient>(c =>
            {
                c.BaseAddress = new Uri(configuration.GetValue<string>("ServiceUrls:APIGateway"));
            });
        }

        private static void RegisterDomainDependencies(IServiceCollection services)
        {
            services.AddTransient<ICityRepository, CityRepository>();
            services.AddTransient<ICityService, CityService>();

            services.AddTransient<IFlightRepository, FlightRepository>();
            services.AddTransient<IFlightService, FlightService>();
        }

        private static void RegisterDatabaseDependencies(IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWorkAdo>();
            services.AddScoped<IDBContext, DBContext>();

            services.AddTransient<IDbConnectionFactory, DbConnectionFactory>();
            services.AddTransient<DbProviderFactory, SqlClientProviderFactory>();
        }
    }
}
