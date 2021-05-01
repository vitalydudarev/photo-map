using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using PhotoMap.Messaging;
using PhotoMap.Messaging.EventHandler;
using PhotoMap.Messaging.EventHandlerManager;
using PhotoMap.Messaging.MessageListener;
using PhotoMap.Messaging.MessageSender;
using PhotoMap.Worker.Handlers;
using PhotoMap.Worker.Services;
using PhotoMap.Worker.Services.Definitions;
using PhotoMap.Worker.Services.Implementations;
using PhotoMap.Worker.Settings;

namespace PhotoMap.Worker
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
            services.AddSingleton<IEventHandler, StartProcessingEventHandler>();
            services.AddSingleton<IEventHandler, PauseProcessingEventHandler>();
            services.AddSingleton<IEventHandlerManager, EventHandlerManager>();

            services.AddSingleton<IMessageListener, RabbitMqMessageListener>();
            services.AddSingleton<IMessageSender2, RabbitMqMessageSender2>();

            // Common services
            services.AddSingleton<IDownloadManager, DownloadManager>();
            services.AddSingleton<IProgressReporter, ProgressReporter>();
            services.AddScoped<IImageProcessingService, ImageProcessingService>();
            services.AddScoped<IImageUploadService, ImageUploadService>();

            // Yandex.Disk services
            services.AddSingleton<IYandexDiskDownloadStateService, YandexDiskDownloadStateService>();
            services.AddScoped<IYandexDiskDownloadService, YandexDiskDownloadService>();

            // Dropbox services
            services.AddSingleton<IDropboxDownloadStateService, DropboxDownloadStateService>();
            services.AddScoped<IDropboxDownloadService, DropboxDownloadService>();

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
