using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PhotoMap.Messaging;
using PhotoMap.Messaging.CommandHandler;
using PhotoMap.Messaging.CommandHandlerManager;
using PhotoMap.Messaging.MessageListener;
using PhotoMap.Messaging.MessageSender;
using Yandex.Disk.Worker.Services;
using Yandex.Disk.Worker.Services.External;

namespace Yandex.Disk.Worker
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
            services.AddHttpClient();

            services.Configure<StorageServiceSettings>(Configuration.GetSection("Storage"));
            services.Configure<RabbitMqSettings>(Configuration.GetSection("RabbitMQ"));
            services.Configure<ImageProcessingSettings>(Configuration.GetSection("ImageProcessing"));

            services.AddScoped(a =>
            {
                var settings = a.GetRequiredService<IOptions<RabbitMqSettings>>().Value;

                return new RabbitMqConfiguration
                {
                    HostName = settings.HostName,
                    Port = settings.Port,
                    UserName = settings.UserName,
                    Password = settings.Password,
                    ConsumeQueueName = settings.CommandsQueueName,
                    ResponseQueueName = settings.ProcessingQueueName
                };
            });

            // register command handlers
            services.AddScoped<ICommandHandler, RunProcessingCommandHandler>();
            services.AddSingleton<IMessageListener, RabbitMqMessageListener>();
            services.AddScoped<IMessageSender, RabbitMqMessageSender>();
            services.AddScoped<ICommandHandlerManager, CommandHandlerManager>();

            services.AddScoped<IYandexDiskDownloadService, YandexDiskDownloadService>();
            services.AddScoped<IStorageService, StorageServiceClient>();
            services.AddHostedService<HostedService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // app.UseAuthorization();

            // app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
