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
            services.Configure<ImageProcessingSettings>(Configuration.GetSection("ImageProcessing"));
            services.Configure<Dictionary<string, RabbitMqSettings>>(Configuration.GetSection("RabbitMQ"));

            services.AddSingleton(a =>
            {
                var settingsDictionary = a.GetRequiredService<IOptions<Dictionary<string, RabbitMqSettings>>>().Value;

                return settingsDictionary.ToDictionary(a => a.Key, b => new RabbitMqConfiguration
                {
                    HostName = b.Value.HostName,
                    Port = b.Value.Port,
                    UserName = b.Value.UserName,
                    Password = b.Value.Password,
                    ConsumeQueueName = b.Value.InQueueName,
                    ResponseQueueName = b.Value.OutQueueName
                });
            });

            services.AddSingleton(a =>
            {
                var settingsDictionary = a.GetRequiredService<IOptions<Dictionary<string, RabbitMqSettings>>>().Value;
                var settings = settingsDictionary.First(key => key.Key == Constants.ImageServiceApi).Value;

                return new RabbitMqConfiguration
                {
                    HostName = settings.HostName,
                    Port = settings.Port,
                    UserName = settings.UserName,
                    Password = settings.Password,
                    ConsumeQueueName = settings.InQueueName,
                    ResponseQueueName = settings.OutQueueName
                };
            });

            // register command handlers
            services.AddSingleton<ICommandHandler, RunProcessingCommandHandler>();
            services.AddSingleton<IMessageListener, RabbitMqMessageListener>();
            services.AddSingleton<IMessageSender2, RabbitMqMessageSender2>();
            services.AddSingleton<ICommandHandlerManager, CommandHandlerManager>();
            services.AddSingleton<DownloadServiceManager>();

            services.AddSingleton<IYandexDiskService, YandexDiskService>();
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
