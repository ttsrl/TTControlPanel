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
using Microsoft.Net.Http.Headers;
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

            services.AddCors(c => { c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin()); });

            services.AddMemoryCache();
            services.AddResponseCaching();
            services.AddControllersWithViews(o => o.Filters.Add<AuthenticationFilter>());

            //session
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.Name = "TTcp_Session";
            });

            //services.AddDbContext<DBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<DBContext>( options => { options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")); }/*, ServiceLifetime.Singleton*/);
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddScoped<Utils>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<Cryptography>();
            services.AddSingleton<GitHub>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDatabaseErrorPage();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error/Index");
            }

            app.UseStatusCodePagesWithReExecute("/Error/Index/{0}");

            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    if (ctx.Context.Response.Headers.ContainsKey(HeaderNames.ContentType) && ctx.Context.Response.Headers[HeaderNames.ContentType] == "application/javascript")
                    {
                        ctx.Context.Response.Headers.Remove(HeaderNames.ContentType);
                        ctx.Context.Response.Headers.Add(HeaderNames.ContentType, "text/javascript; charset=utf-8");
                    }
                    if (ctx.Context.Response.Headers.ContainsKey(HeaderNames.ContentType) && ctx.Context.Response.Headers[HeaderNames.ContentType] == "text/css")
                    {
                        ctx.Context.Response.Headers.Remove(HeaderNames.ContentType);
                        ctx.Context.Response.Headers.Add(HeaderNames.ContentType, "text/css; charset=utf-8");
                    }
                    if (!ctx.Context.Response.Headers.ContainsKey("X-Content-Type-Options"))
                        ctx.Context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                    ctx.Context.Response.Headers[HeaderNames.CacheControl] = "immutable,max-age=" + TimeSpan.FromDays(365).TotalSeconds;
                }
            });

            app.UseRouting();

            app.UseResponseCaching();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseSession();

            app.Use(async (context, next) =>
            {
                if (!context.Response.Headers.ContainsKey("X-Content-Type-Options"))
                    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                await next();
            });

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
