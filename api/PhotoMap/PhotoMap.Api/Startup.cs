using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using PhotoMap.Api.Database;
using PhotoMap.Api.ServiceClients.StorageService;
using PhotoMap.Api.Services;
using PhotoMap.Messaging;
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

            services.AddScoped(a =>
            {
                var settings = a.GetRequiredService<IOptions<RabbitMqSettings>>().Value;

                return new RabbitMqConfiguration
                {
                    HostName = settings.HostName,
                    Port = settings.Port,
                    UserName = settings.UserName,
                    Password = settings.Password,
                    ConsumeQueueName = settings.ResultsQueueName
                };
            });

            services.AddHttpClient();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IThumbnailService, ThumbnailService>();

            services.AddHostedService<HostedService>();

            services.AddScoped<IMessageSender, RabbitMqMessageSender>();
            services.AddScoped<IMessageListener, RabbitMqMessageListener>();
            services.AddScoped<ICommandHandlerManager, CommandHandlerManager>();
            services.AddScoped<Database.Services.IUserService, Database.Services.UserService>();
            services.AddScoped<IStorageService, StorageServiceClient>();

            services.AddDbContext<PhotoMapContext>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PhotoMap API V1", Version = "v1" });
            });
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

            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "PhotoMap API V1");
            });
        }
    }
}
