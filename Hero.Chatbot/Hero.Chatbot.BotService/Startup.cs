// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EmptyBot v4.14.0

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Hero.Chatbot.BotService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();

            services.AddApplicationInsightsTelemetry(options =>
            {
                options.InstrumentationKey = Configuration["ApplicationInsightsInstrumentationKey"];
                options.EndpointAddress = Configuration["ApplicationInsightsEndpointAddress"];
            });

            services.AddLogging(builder =>
            {
                builder.Services.AddApplicationInsightsTelemetry(options =>
                {
                    options.InstrumentationKey = Configuration["ApplicationInsightsInstrumentationKey"];
                    options.EndpointAddress = Configuration["ApplicationInsightsEndpointAddress"];
                });
            });

            services.AddCors(a => a.AddPolicy("allowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

            services.RegisterInjections();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("allowAll");

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseWebSockets()
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });

            // app.UseHttpsRedirection();
        }
    }
}
