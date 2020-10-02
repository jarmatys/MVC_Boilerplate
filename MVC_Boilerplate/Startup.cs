using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MVC_Boilerplate.Context;
using MVC_Boilerplate.Models.Db.Account;
using MVC_Boilerplate.Services;
using MVC_Boilerplate.Services.Interfaces;

namespace MVC_Boilerplate
{
    public class Startup
    {
        // Wstrzykiwanie pliku konfiguracyjnego do naszej aplikacji
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            var config = new ConfigurationBuilder();
            config.AddJsonFile("secrets.json");
            Configuration = config.Build();
            // Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // £¹czenie z baz¹ danych
            services.AddDbContext<DBContext>(builder =>
            {
                // Dodajemy dostawcê do obs³ugi MySql'a i przekazujemy connection string pobrany z pliku konfiguracyjnego z naszej aplikacji
                builder.UseMySql(Configuration.GetConnectionString("DefaultConnection"));
            });

            // Wstrzykujemy zale¿noœci o identifykacji u¿ytkowników
            services.AddIdentity<User, IdentityRole>(options =>
            {
                // Opcje dotycz¹ce has³a
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 2;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;

            }).AddEntityFrameworkStores<DBContext>();

            // Dodajemy obs³ugê silnika razor oraz controllerów
            services.AddRazorPages();

            services.AddControllersWithViews();

            // Tutaj dodajemy zale¿noœci do wstrzykiwania
            services.AddScoped<IHomeService, HomeService>();
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
                app.UseExceptionHandler("/blad");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // uruchamianie customowych stron b³êdów w controllerze ErrorController
            app.UseStatusCodePagesWithReExecute("/error/{0}");

            // przekazywanie nag³ówków aplikacji do reverse proxy
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // w³¹czamy u¿ywanie autoryzacji i autentykacji z .NET Core Identity
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
