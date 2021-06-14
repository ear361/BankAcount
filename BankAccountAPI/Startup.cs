using System;
using BankAccountAPI.Middlewares;
using BankAccountAPI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BankAccountAPI
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
            var appConfig = new AppConfig();
            Configuration.GetSection(nameof(AppConfig)).Bind(appConfig);
            services.AddControllers()
                .AddNewtonsoftJson();
            ;
            services.AddEntityFrameworkSqlite().AddDbContext<BankContext>(
                options =>
                {
                    //todo: move connection string to appsettings
                    options.UseSqlite(appConfig.ConnectionString);
                });;

            services.AddHttpClient("APIGateWayClient", c =>
            {
                c.DefaultRequestHeaders.Add("Accept", "application/json");
                c.BaseAddress = new Uri(appConfig.ConnectionString);
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}