using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TTControlPanel.Filters;
using TTControlPanel.Services;


namespace TTControlPanel
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddCors(c => { c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin()); });
            services.AddControllersWithViews(options => { options.Filters.Add<AuthenticationFilter>(); });
            services.AddDistributedMemoryCache();
            services.AddSession(s =>
            {
                s.IdleTimeout = TimeSpan.FromMinutes(20);
                s.Cookie.Name = "cpanelTT_Session";
            });

            services.AddDbContext<DBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddSingleton<Utils>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<Cryptography>();
            services.AddSingleton<GitHub>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseDatabaseErrorPage();
            }
            app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");
            app.UseStaticFiles();
            app.UseSession();
            app.UseCookiePolicy();
            app.UseRouting();
            app.UseAuthorization();
            app.UseCors(options => options.AllowAnyOrigin());
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });


            var cultureInfo = new CultureInfo("it-IT");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            CultureInfo.CurrentCulture = cultureInfo;
            // Configure the Localization middleware
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(cultureInfo),
                SupportedCultures = new List<CultureInfo>
                {
                    cultureInfo
                },
                SupportedUICultures = new List<CultureInfo>
                {
                    cultureInfo
                }
            });
        }
    }
}
