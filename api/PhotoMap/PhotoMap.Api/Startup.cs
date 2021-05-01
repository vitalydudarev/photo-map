using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using PhotoMap.Api.Database;
using PhotoMap.Api.Handlers;
using PhotoMap.Api.Hubs;
using PhotoMap.Api.Middlewares;
using PhotoMap.Api.Services;
using PhotoMap.Api.Services.Implementations;
using PhotoMap.Api.Services.Interfaces;
using PhotoMap.Api.Settings;
using PhotoMap.Messaging;
using PhotoMap.Messaging.EventHandler;
using PhotoMap.Messaging.EventHandlerManager;
using PhotoMap.Messaging.MessageListener;
using PhotoMap.Messaging.MessageSender;
using Serilog;

namespace PhotoMap.Api
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
            services.AddControllers();
            services.Configure<RabbitMqSettings>(Configuration.GetSection("RabbitMQ"));
            services.Configure<StorageServiceSettings>(Configuration.GetSection("Storage"));
            services.Configure<YandexDiskFileProviderSettings>(Configuration.GetSection("YandexDiskFileProvider"));

            services.AddSingleton(provider => new UserInfo { UserId = 1, Name = "Vitaly" });

            services.AddSingleton(a =>
            {
                var settings = a.GetRequiredService<IOptions<RabbitMqSettings>>().Value;

                return new RabbitMqConfiguration
                {
                    HostName = settings.HostName,
                    Port = settings.Port,
                    UserName = settings.UserName,
                    Password = settings.Password,
                    ConsumeQueueName = settings.ResultsQueueName,
                    ResponseQueueName = settings.CommandsQueueName
                };
            });

            services.AddHttpClient();

            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<IUserService, UserService>();

            services.AddHostedService<HostedService>();

            services.AddSingleton<YandexDiskHub>();
            services.AddSingleton<DropboxHub>();

            services.AddSingleton<IEventHandler, ProgressMessageHandler>();
            services.AddSingleton<IEventHandler, ImageProcessedEventHandler>();
            services.AddSingleton<IEventHandler, NotificationHandler>();
            services.AddSingleton<IMessageSender, RabbitMqMessageSender>();
            services.AddSingleton<IMessageListener, RabbitMqMessageListener>();
            services.AddSingleton<IEventHandlerManager, EventHandlerManager>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IStorageService, StorageServiceClient>();
            services.AddScoped<HostInfo>();
            services.AddScoped<IFileProvider, LocalFileProvider>();
            services.AddSingleton<IConvertedImageHolder, ConvertedImageHolder>();

            services.AddDbContext<PhotoMapContext>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PhotoMap API V1", Version = "v1" });
            });

            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<HostInfoMiddleware>();
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseHttpsRedirection();

            app.UseSerilogRequestLogging();

            app.UseCors(builder => builder
                .WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<YandexDiskHub>("/yandex-disk-hub");
                endpoints.MapHub<DropboxHub>("/dropbox-hub");
                endpoints.MapControllers();
            });

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "PhotoMap API V1");
            });
        }
    }
}
