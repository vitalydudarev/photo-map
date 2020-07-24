using System;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Storage.Service.Database;
using Storage.Service.Database.Repository;
using Storage.Service.Service;
using Storage.Service.Storage;

namespace Storage.Service
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
            services.Configure<FileStorageSettings>(Configuration.GetSection("FileStorage"));
            services.AddTransient<IFileStorage, FileStorage>();
            services.AddTransient<IFileRepository, FileRepository>();
            services.AddTransient<IFileService, FileService>();
            services.AddDbContext<FileContext>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PhotoMap.Storage.Service API V1", Version = "v1" });
            });

            services.AddAutoMapper(expression => expression.AddProfile(new MappingProfile()), new Type[0]);
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

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "PhotoMap.Storage.Service API V1");
            });
        }
    }
}
