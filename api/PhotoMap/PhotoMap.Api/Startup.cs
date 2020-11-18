using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using PhotoMap.Api.Database;
using PhotoMap.Api.Database.Services;
using PhotoMap.Api.Handlers;
using PhotoMap.Api.Hubs;
using PhotoMap.Api.ServiceClients.StorageService;
using PhotoMap.Api.Services;
using PhotoMap.Api.Settings;
using PhotoMap.Messaging;
using PhotoMap.Messaging.CommandHandler;
using PhotoMap.Messaging.CommandHandlerManager;
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

            services.AddSingleton<ICommandHandler, ProgressMessageHandler>();
            services.AddSingleton<ICommandHandler, ResultsCommandHandler>();
            services.AddSingleton<ICommandHandler, YandexDiskNotificationHandler>();
            services.AddSingleton<IMessageSender, RabbitMqMessageSender>();
            services.AddSingleton<IMessageListener, RabbitMqMessageListener>();
            services.AddSingleton<ICommandHandlerManager, CommandHandlerManager>();
            services.AddScoped<Database.Services.IUserService, Database.Services.UserService>();
            services.AddScoped<IStorageService, StorageServiceClient>();

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
