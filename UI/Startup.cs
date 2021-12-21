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
using Microsoft.AspNetCore.DataProtection;

namespace UI
{
    public class Startup
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        public Startup(IWebHostEnvironment env, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = new ConfigurationBuilder()
                 .SetBasePath(env.ContentRootPath)
                 .AddJsonFile("appsettings.json", false, true)
                 .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
                 .Build();


            _hostingEnvironment = hostingEnvironment;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataProtection()
                .PersistKeysToFileSystem(new System.IO.DirectoryInfo(Configuration.GetSection("Authentication")["KeyPath"]))                
                .SetApplicationName(Configuration.GetSection("Authentication")["AppName"]);                
            
            services.AddAuthentication("Identity.Application")
                 .AddCookie("Identity.Application", config =>
                 {
                     config.Cookie.HttpOnly = true;
                     config.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                     config.Cookie.SameSite = SameSiteMode.Lax;
                     config.SlidingExpiration = true;
                     config.ExpireTimeSpan = TimeSpan.FromHours(24);
                     config.Cookie.Name = Configuration.GetSection("Authentication")["CookieName"];
                     config.Cookie.Path = "/";
                     config.Cookie.Domain = Configuration.GetSection("Authentication")["Domain"];
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

            
            var strLogFolder = Configuration.GetSection("Folders")["Log"];
            if (string.IsNullOrEmpty(strLogFolder))
            {
                strLogFolder = System.IO.Directory.GetCurrentDirectory() + "\\Logs";
            }

            var execAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            var versionTime = new System.IO.FileInfo(execAssembly.Location).LastWriteTime;

            services.AddSingleton<BL.RunningApp>(x => new BL.RunningApp()
            {
                ConnectString = Configuration.GetSection("ConnectionStrings")["AppConnection"]
                ,
                AppName = Configuration.GetSection("App")["Name"]
                ,
                AppVersion = Configuration.GetSection("App")["Version"]
                ,
                FulltextSearch= BO.BAS.BG(Configuration.GetSection("App")["FulltextSearch"])
                ,
                AppBuild = "build: " + BO.BAS.ObjectDateTime2String(versionTime)
                ,
                LogoImageSuffix = Configuration.GetSection("App")["LogoImageSuffix"]                                             
                ,
                Implementation = Configuration.GetSection("App")["Implementation"]
                ,
                DefaultLangIndex = BO.BAS.InInt(Configuration.GetSection("App")["DefaultLangIndex"])
                ,
                LangChooseIsStopped = BO.BAS.BG(Configuration.GetSection("App")["LangChooseIsStopped"])
                ,
                CssCustomSkin = Configuration.GetSection("App")["CssCustomSkin"]
                ,
                UserUrl= Configuration.GetSection("App")["UserUrl"]
                ,
                RobotUser = Configuration.GetSection("App")["RobotUser"]
                ,
                RobotIsStopped= BO.BAS.BG(Configuration.GetSection("App")["RobotIsStopped"])
                ,
                UploadFolder = Configuration.GetSection("Folders")["Upload"]
                ,
                TempFolder = Configuration.GetSection("Folders")["Temp"]
                ,                
                ReportFolder = Configuration.GetSection("Folders")["Report"]
                ,
                LogFolder = strLogFolder
                ,
                AppRootFolder= _hostingEnvironment.ContentRootPath
                ,
                TranslatorMode = Configuration.GetSection("App")["TranslatorMode"]
                ,
                UiftUrl= Configuration.GetSection("UIFT")["Url"]
                ,
                PasswordMinLength= Convert.ToInt32(Configuration.GetSection("PasswordChecker")["MinLength"])
                ,
                PasswordMaxLength = Convert.ToInt32(Configuration.GetSection("PasswordChecker")["MaxLength"])
                ,
                PasswordRequireDigit= Convert.ToBoolean(Configuration.GetSection("PasswordChecker")["RequireDigit"])
                ,
                PasswordRequireLowercase = Convert.ToBoolean(Configuration.GetSection("PasswordChecker")["RequireLowercase"])
                ,
                PasswordRequireUppercase = Convert.ToBoolean(Configuration.GetSection("PasswordChecker")["RequireUppercase"])
                ,
                PasswordRequireNonAlphanumeric = Convert.ToBoolean(Configuration.GetSection("PasswordChecker")["RequireNonAlphanumeric"])
                ,
                PipeBaseUrl = Configuration.GetSection("Pipe")["BaseUrl"]
                ,
                PipeIsActive = BO.BAS.BG(Configuration.GetSection("Pipe")["IsActive"])
            });

            services.AddHttpClient();       //kvùli httpclient

            services.AddSingleton<BL.TheEntitiesProvider>();
            services.AddSingleton<BL.TheTranslator>();
            services.AddSingleton<BL.TheColumnsProvider>();
            services.AddSingleton<BL.ThePeriodProvider>();
            services.AddSingleton<BL.TheGlobalParams>();
            
            services.TryAddSingleton<IReportServiceConfiguration>(sp =>
            new ReportServiceConfiguration
            {
                ReportingEngineConfiguration = ConfigurationHelper.ResolveConfiguration(sp.GetService<IWebHostEnvironment>()),HostAppId = "ReportingCore3App",Storage = new Telerik.Reporting.Cache.File.FileStorage()
                //ReportSourceResolver = new UriReportSourceResolver(conf.GetSection("Folders")["Report"])    //.AddFallbackResolver(new TypeReportSourceResolver())
            });
           
            services.AddScoped<BO.RunningUser, BO.RunningUser>();
            services.AddScoped<BL.Factory, BL.Factory>();

            services.AddHostedService<UI.TheRobot>();
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
            
            //app.UseHttpsRedirection();
            
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();    //user-identity funguje pouze s app.UseAuthentication
            app.UseAuthorization();

            app.UseRequestLocalization();

            var strCultureCode = Configuration.GetSection("App")["CultureCode"];
            if (string.IsNullOrEmpty(strCultureCode)) strCultureCode = "cs-CZ";
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = new System.Globalization.CultureInfo(strCultureCode);
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = new System.Globalization.CultureInfo(strCultureCode);



            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers(); //kvùli teleri reporting
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
