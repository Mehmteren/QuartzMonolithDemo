using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using QuartzMonolithDemo.Services;
using QuartzMonolithDemo.Services.Interface;
using QuartzMonolithDemo.Jobs;

namespace QuartzMonolithDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // Dependency Injection for services
            services.AddSingleton<ILogManager, LogManager>();
            services.AddSingleton<IJobMonitor, JobMonitorService>();

            // Quartz job and scheduler
            services.AddTransient<LogJob>();
            services.AddHostedService<QuartzSchedulerService>();

            // Swagger configuration
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "QuartzMonolithDemo - NLB Ready",
                    Version = "v1",
                    Description = "Minimal NLB Cluster ready .NET Core 5 service with Quartz scheduling"
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "QuartzMonolithDemo v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
