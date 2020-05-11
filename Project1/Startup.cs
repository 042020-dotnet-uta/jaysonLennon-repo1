using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using StoreApp.Data;

namespace StoreApp.StartupUtil
{
    using StoreApp.Repository;

    public static class Repos
    {
        public static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped<ICustomer, CustomerRepository>();
            services.AddScoped<ILocationRepository, LocationRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
        }
    }
}
namespace StoreApp
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
            services.AddControllersWithViews();

            services.AddDbContext<StoreContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("StoreApp")));
            
            services.AddLogging(logger => 
            {
                Host.CreateDefaultBuilder()
                .ConfigureLogging(logging => {
                    logging.ClearProviders();
                    logging.AddConsole();
                });
            });

            StartupUtil.Repos.AddRepositories(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
