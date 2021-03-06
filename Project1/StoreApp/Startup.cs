using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.Cookies;

using StoreApp.Data;
using StoreApp.Business;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace StoreApp.StartupUtil
{
    using StoreApp.Repository;

    /// <summary>
    /// Helper class to add repos.
    /// </summary>
    public static class Repos
    {
        /// <summary>
        /// Add all repositories to the service.
        /// </summary>
        /// <param name="services">Service to add to.</param>
        public static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped<IUser, UserRepository>();
            services.AddScoped<ILocation, LocationRepository>();
            services.AddScoped<IOrder, OrderRepository>();
            services.AddScoped<IProduct, ProductRepository>();
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

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options =>
                    {
                        options.Cookie.HttpOnly = true;
                        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);

                        options.LoginPath = "/Account/Login";
                        options.AccessDeniedPath = "/Account/AccessDenied";
                        options.SlidingExpiration = true;
                    });

            StartupUtil.Repos.AddRepositories(services);

            services.AddScoped<PageHeader.PopulateHeader>();
            services.AddScoped<FlashMessage.FlashMessageFilter>();
            services.AddScoped<IBusinessRules, BusinessRules>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
                app.UseExceptionHandler("/Error");
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            var cookiePolicyOptions = new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.Strict,
            };

            app.UseCookiePolicy(cookiePolicyOptions);

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
