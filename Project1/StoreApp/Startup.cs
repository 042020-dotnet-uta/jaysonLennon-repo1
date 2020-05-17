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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace StoreApp.StartupUtil
{
    using StoreApp.Repository;

    public static class Repos
    {
        public static void AddRepositories(IServiceCollection services)
        {
<<<<<<< HEAD:Project1/Startup.cs
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ILocationRepository, LocationRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
=======
            services.AddScoped<IUser, UserRepository>();
            services.AddScoped<ILocation, LocationRepository>();
            services.AddScoped<IOrder, OrderRepository>();
            services.AddScoped<IProduct, ProductRepository>();
>>>>>>> develop:Project1/StoreApp/Startup.cs
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

<<<<<<< HEAD:Project1/Startup.cs
            StartupUtil.Repos.AddRepositories(services);
=======
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

            services.AddScoped<CartHeader.CartHeaderFilter>();
            services.AddScoped<FlashMessage.FlashMessageFilter>();
>>>>>>> develop:Project1/StoreApp/Startup.cs
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseExceptionHandler("/Error");
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // IMPORTANT: These have to be called in this specific order.

            app.UseHttpsRedirection();
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
