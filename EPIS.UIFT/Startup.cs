using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UIFT.Security;
using rep = UIFT.Repository;

namespace UIFT
{
    public class Startup
    {
        private AppConfiguration AppConfig;
        public IConfigurationRoot Configuration { get; }
        
        public Startup(IWebHostEnvironment env)
        {
            Configuration = new ConfigurationBuilder()
                 .SetBasePath(env.ContentRootPath)
                 .AddJsonFile("appsettings.json", false, true)
                 .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
                 .Build();

            AppConfig = new AppConfiguration();
            Configuration.Bind(AppConfig);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // configuration
            services.AddSingleton(AppConfig);

            services.AddResponseCaching();
            services.AddLocalization();

            services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                    options.UseMemberCasing();
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                });

            // CSRF
            services.AddAntiforgery(options =>
            {
                options.FormFieldName = "__UiftCsrfToken";
                options.HeaderName = "UiftCsrf";
            });

            // autentizace
            services.AddDataProtection()
                .PersistKeysToFileSystem(new System.IO.DirectoryInfo(AppConfig.Authentication.KeyPath))
                .SetApplicationName(AppConfig.Authentication.AppName);

            services
                .AddAuthentication("Identity.Application")
                .AddCookie("Identity.Application", config =>
                {
                    config.Cookie.Name = AppConfig.Authentication.CookieName;
                    config.Cookie.Domain = AppConfig.Authentication.Domain;
                    config.Cookie.Path = "/";
                    config.Cookie.HttpOnly = true;
                    config.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                    config.Cookie.SameSite = SameSiteMode.Lax;
                    if (!string.IsNullOrEmpty(AppConfig.Authentication.Domain))
                        config.Cookie.Domain = AppConfig.Authentication.Domain;
                    config.ReturnUrlParameter = "returnurl";
                    config.LoginPath = "/Login";
                });

            var execAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            var versionTime = new System.IO.FileInfo(execAssembly.Location).LastWriteTime;

            services.AddSingleton<BL.RunningApp>(x => new BL.RunningApp()
            {
                ConnectString = Configuration.GetSection("ConnectionStrings")["AppConnection"],
                AppName = "UIFT",
                AppVersion = "version 1.0",
                AppBuild = "build: " + BO.BAS.ObjectDateTime2String(versionTime),
                Implementation = "DEFAULT",
                UploadFolder = AppConfig.UploadFolder,
                TempFolder = AppConfig.TempFolder,
                LogFolder = AppConfig.LogFolder,
                TranslatorMode = "Collect"
            });


            services.AddSingleton<BL.TheEntitiesProvider>();
            services.AddSingleton<BL.TheTranslator>();
            services.AddSingleton<BL.TheColumnsProvider>();
            services.AddSingleton<BL.ThePeriodProvider>();
            services.AddSingleton<BL.TheGlobalParams>();

            services.AddScoped<BO.RunningUser, BO.RunningUser>();
            services.AddScoped<BL.Factory, BL.Factory>();
            
            services.AddScoped<rep.RepositoryFactory>();
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
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseRequestLocalization(opt =>
            {
                opt.SetDefaultCulture("cs-CZ");
                opt.AddSupportedCultures("cs-CZ");
                opt.AddSupportedUICultures("cs-CZ");
            });
            
            app.Use(async (context, next) =>
            {
                context.Response.GetTypedHeaders().CacheControl =
                    new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
                    {
                        NoCache = true,
                        NoStore = true,
                        SharedMaxAge = TimeSpan.FromSeconds(0)
                    };
                await next();
            });

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseUiftAuth();

            app.UseEndpoints(endpoints =>
            {
                // error route
                endpoints.MapControllerRoute(
                    name: "error",
                    pattern: "error/{code}",
                    defaults: new { controller = "Error", action = "Index" },
                    constraints: new { code = @"\d+" }
                );

                // default - logovaci dialog
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "",
                    defaults: new { controller = "Login", action = "Index" }
                );

                // zobrazeni formulare
                endpoints.MapControllerRoute(
                    name: "form",
                    pattern: "Formular/{a11id}/{sekce?}/{otazka?}",
                    defaults: new { controller = "Formular", action = "Index" },
                    constraints: new { a11id = @"\d+", sekce = @"^$|\d+", otazka = @"^$|\d+" }
                );

                // preview otazek a sekci
                endpoints.MapControllerRoute(
                    name: "preview",
                    pattern: "Preview/{action}/{a11id}",
                    defaults: new { controller = "Preview" },
                    constraints: new { a11id = @"\d+", action = "Formular|Sekce|Otazka" }
                );

                // export
                endpoints.MapControllerRoute(
                    name: "export",
                    pattern: "Export/{action}/{a11id}",
                    defaults: new { controller = "Export" },
                    constraints: new { action = "ToWord", a11id = @"\d+" }
                );

                endpoints.MapControllerRoute(
                    name: "ajax_calls",
                    pattern: "{a11id}/{controller}/{action}/{id?}",
                    defaults: new { action = "Index" },
                    constraints: new { a11id = @"\d+", id = @"^$|\d+" }
                );

                endpoints.MapControllerRoute(
                    name: "specials",
                    pattern: "{controller}/{action}/{id?}",
                    defaults: new { action = "Index" },
                    constraints: new { controller = "alogin|Login|Export" }
                );
            });
        }
    }
}
