using Hero.Chatbot.BotService.Dialogs;
using Hero.Chatbot.BotService.Stores;
using Hero.Chatbot.Repository;
using Hero.Chatbot.Repository.Contracts;
using Hero.Chatbot.Service;
using Hero.Chatbot.Service.Contracts;
using Hero.Shared.DbContext;
using Hero.Shared.Repository;
using Hero.Shared.UnitOfWork;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;

namespace Hero.Chatbot.BotService
{
    public static class ServiceConfigExtensions
    {
        public static void RegisterInjections(this IServiceCollection services)
        {
            RegisterDatabaseDependencies(services);
            RegisterDomainDependencies(services);
            RegisterChatbotDependencies(services);
        }

        private static void RegisterDomainDependencies(IServiceCollection services)
        {
            services.AddTransient<IFlightRepository, FlightRepository>();
            services.AddTransient<IFlightService, FlightService>();

            services.AddTransient<IPromotionRepository, PromotionRepository>();
            services.AddTransient<IPromotionService, PromotionService>();

            services.AddTransient<IExtraServiceRepository, ExtraServiceRepository>();
            services.AddTransient<IExtraServiceService, ExtraServiceService>();
        }

        private static void RegisterDatabaseDependencies(IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWorkAdo>();
            services.AddScoped<IDBContext, DBContext>();

            services.AddTransient<IDbConnectionFactory, DbConnectionFactory>();
            services.AddTransient<DbProviderFactory, SqlClientProviderFactory>();
        }

        private static void RegisterChatbotDependencies(IServiceCollection services)
        {
            // Create the Bot Framework Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            // Bot states
            services.AddSingleton<IStorage, BotStorage>();
            services.AddSingleton<UserState>();
            services.AddSingleton<ConversationState>();

            // Recognizers
            services.AddSingleton<FlightBookingRecognizer>();

            // Dialogs
            services.AddScoped<FlightSearchDialog>();
            services.AddScoped<FlightManagementDialog>();
            services.AddScoped<GeneralInformationDialog>();
            services.AddScoped<PromotionDialog>();
            services.AddScoped<MoreOptionDialog>();
            services.AddScoped<MainDialog>();

            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddTransient<IBot, FlightBookingBot<MainDialog>>();
        }
    }
}
