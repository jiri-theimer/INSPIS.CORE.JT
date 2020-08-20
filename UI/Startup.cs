using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Telerik.Reporting.Services;
using Telerik.Reporting.Services.AspNetCore;

namespace UI
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
            services.AddAuthentication("CookieAuthentication")
                 .AddCookie("CookieAuthentication", config =>
                 {
                     config.Cookie.HttpOnly = true;
                     config.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                     config.Cookie.SameSite = SameSiteMode.None;
                     config.SlidingExpiration = true;
                     config.ExpireTimeSpan = TimeSpan.FromHours(24);
                     config.Cookie.Name = "InspisCloudCore";
                     config.ReturnUrlParameter = "returnurl";
                     config.LoginPath = "/Login/UserLogin";
                 });

            services.Configure<Microsoft.Extensions.WebEncoders.WebEncoderOptions>(options =>
            {
                options.TextEncoderSettings = new System.Text.Encodings.Web.TextEncoderSettings(System.Text.Unicode.UnicodeRanges.All);
            });

            services.AddControllers();      //kvùli telerik reporting
            services.AddControllersWithViews();
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;      //kvùli telerik reporting
            });

            services.AddRazorPages().AddNewtonsoftJson();   //kvùli telerik reporting

            var conf = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var strLogFolder = conf.GetSection("Folders")["Log"];
            if (string.IsNullOrEmpty(strLogFolder))
            {
                strLogFolder = System.IO.Directory.GetCurrentDirectory() + "\\Logs";
            }

            var execAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            var versionTime = new System.IO.FileInfo(execAssembly.Location).LastWriteTime;

            services.AddSingleton<BL.RunningApp>(x => new BL.RunningApp()
            {
                ConnectString = conf.GetSection("ConnectionStrings")["AppConnection"]
                ,
                AppName = conf.GetSection("App")["Name"]
                ,
                AppVersion = conf.GetSection("App")["Version"]
                ,
                AppBuild = "build: " + BO.BAS.ObjectDateTime2String(versionTime)
                ,
                LogoImage = conf.GetSection("App")["LogoImage"]
                ,
                Implementation = conf.GetSection("App")["Implementation"]
                ,
                UploadFolder = conf.GetSection("Folders")["Upload"]
                ,
                TempFolder = conf.GetSection("Folders")["Temp"]
                ,
                ReportFolder = conf.GetSection("Folders")["Report"]
                ,
                LogFolder = strLogFolder
                ,
                TranslatorMode = conf.GetSection("App")["TranslatorMode"]
            });


            services.AddSingleton<BL.TheEntitiesProvider>();
            services.AddSingleton<BL.TheTranslator>();
            services.AddSingleton<BL.TheColumnsProvider>();
            services.AddSingleton<BL.ThePeriodProvider>();
            services.AddSingleton<BL.TheGlobalParams>();
            
            services.TryAddSingleton<IReportServiceConfiguration>(sp =>
            new ReportServiceConfiguration
            {
                ReportingEngineConfiguration = ConfigurationHelper.ResolveConfiguration(sp.GetService<IWebHostEnvironment>()),HostAppId = "ReportingCore3App",Storage = new Telerik.Reporting.Cache.File.FileStorage(),
                ReportSourceResolver = new UriReportSourceResolver(conf.GetSection("Folders")["Report"])
            });
           
            services.AddScoped<BO.RunningUser, BO.RunningUser>();
            services.AddScoped<BL.Factory, BL.Factory>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //    app.UseHsts();
            //}
            app.UseDeveloperExceptionPage();    //zjt: v rámci vývoje

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();    //user-identity funguje pouze s app.UseAuthentication
            app.UseAuthorization();

            app.UseRequestLocalization();
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = new System.Globalization.CultureInfo("cs-CZ");
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = new System.Globalization.CultureInfo("cs-CZ");


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });



            loggerFactory.AddFile("Logs/info-{Date}.log", LogLevel.Information);
            loggerFactory.AddFile("Logs/debug-{Date}.log", LogLevel.Debug);
            loggerFactory.AddFile("Logs/error-{Date}.log", LogLevel.Error);

        }
    }
}
