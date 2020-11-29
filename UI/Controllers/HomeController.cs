using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Diagnostics;

using Microsoft.AspNetCore.Hosting;

namespace UI.Controllers
{

    

    public class HomeController : BaseController
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
       
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment hostingEnvironment)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<IActionResult> Logout()
        {
            //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync("Identity.Application");
            
            return View();

        }

        public BO.Result SaveCurrentUserHomePage(string homepageurl)
        {
            if (!String.IsNullOrEmpty(homepageurl))
            {
                if (homepageurl.Substring(0, 1) != "/")
                {
                    homepageurl = "/" + homepageurl;
                }
                if (homepageurl.Contains("/RecPage"))
                {   //ořezat parametry za otazníkem
                    homepageurl = homepageurl.Split("?")[0];
                }
                else
                {
                    homepageurl = homepageurl.Replace("pid=", "xxx=");
                }
            }
            
            var c = Factory.j03UserBL.Load(Factory.CurrentUser.pid);
            c.j03HomePageUrl = homepageurl;
            Factory.j03UserBL.Save(c);
            return new BO.Result(false);
        }
        public BO.Result SaveCurrentUserFontStyle(int fontstyleflag)
        {
            var c = Factory.j03UserBL.Load(Factory.CurrentUser.pid);
            c.j03FontStyleFlag = fontstyleflag;
            Factory.j03UserBL.Save(c);
            return new BO.Result(false);
        }
        public BO.Result SaveCurrentUserLangIndex(int langindex)
        {
            var c = Factory.j03UserBL.Load(Factory.CurrentUser.pid);
            c.j03LangIndex = langindex;
            Factory.j03UserBL.Save(c);
            var co = new CookieOptions() { Expires = DateTime.Now.AddDays(100) };
            Response.Cookies.Append("inspis.core.langindex", langindex.ToString(), co);
            return new BO.Result(false);
        }
        public BO.Result SaveCurrentUserLogoVisibility(string isvisible)
        {
            var c = Factory.j03UserBL.Load(Factory.CurrentUser.pid);
            c.j03IsMainLogoVisible = BO.BAS.BG(isvisible);
            Factory.j03UserBL.Save(c);
            return new BO.Result(false);
        }

        public BO.Result UpdateCurrentUserPing(BO.j92PingLog c)
        {
            var uaParser = UAParser.Parser.GetDefault();
            UAParser.ClientInfo client_info = uaParser.Parse(c.j92BrowserUserAgent);
            c.j92BrowserOS = client_info.OS.Family + " " + client_info.OS.Major;
            c.j92BrowserFamily = client_info.UA.Family + " " + client_info.UA.Major;
            c.j92BrowserDeviceFamily = client_info.Device.Family;
            
            Factory.j03UserBL.UpdateCurrentUserPing(c);
            
            return new BO.Result(false);
        }

        public BO.Result StartStopLiveChat(int flag)    //flag:1=zapnout smartsupp
        {
            var c = Factory.j03UserBL.Load(Factory.CurrentUser.pid);
            if (flag == 1)
            {
                c.j03LiveChatTimestamp = DateTime.Now;   //zapnout smartsupp
            }
            else
            {
                c.j03LiveChatTimestamp = null;   //vypnout smartsupp
            }
            Factory.j03UserBL.Save(c);
            return new BO.Result(false);
        }
        public IActionResult LiveChat()
        {
            
            return View();
        }

        public IActionResult Index()
        {
            bool bolRedirect = false;
            if (HttpContext.Request.Path.Value.Length <= 1)
            {
                //úvodní spuštění: otestovat nastavení domovské stránky
                if (Factory.CurrentUser.j03HomePageUrl != null)
                {
                    bolRedirect = true;
                    Response.Redirect(Factory.CurrentUser.getHomePageUrl());
                    
                }
            }

            var v = new HomeViewModel();
            if (!bolRedirect)
            {                
                var pandulak = new ThePandulak(_hostingEnvironment);
                v.Pandulak1 = pandulak.getPandulakImage(1);
                v.Pandulak2 = pandulak.getPandulakImage(2);                
            }
            return View(v);

        }

        public IActionResult About()
        {
            
            return View();
        }
     
        public IActionResult MyProfile()
        {           
            var v = new MyProfileViewModel();
            v.userAgent = Request.Headers["User-Agent"];
            
            var uaParser = UAParser.Parser.GetDefault();
            v.client_info = uaParser.Parse(v.userAgent);
            v.RecJ02 = Factory.j02PersonBL.Load(Factory.CurrentUser.j02ID);
            v.RecJ03 = Factory.j03UserBL.Load(Factory.CurrentUser.pid);
            v.CurrentUser = Factory.CurrentUser;
            if (v.CurrentUser.j03GridSelectionModeFlag == 1)
            {
                v.IsGridClipboard = true;
            }
            v.EmailAddres = v.RecJ02.j02Email;

            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MyProfile(Models.MyProfileViewModel v)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(v.EmailAddres) == true)
                {
                    this.AddMessage("Chybí e-mail adresa.");
                    return MyProfile();
                }
                BO.j02Person c = Factory.j02PersonBL.Load(Factory.CurrentUser.j02ID);
                c.j02Email = v.EmailAddres;
                if (Factory.j02PersonBL.Save(c) > 0)
                {
                    BO.j03User cUser = Factory.j03UserBL.Load(Factory.CurrentUser.pid);
                    if (v.IsGridClipboard == true)
                    {
                        cUser.j03GridSelectionModeFlag = 1;
                    }
                    else
                    {
                        cUser.j03GridSelectionModeFlag = 0;
                    }
                    Factory.j03UserBL.Save(cUser);
                    Factory.CurrentUser.AddMessage("Změny uloženy", "info");
                }

            }            
            return MyProfile();
        }

        public IActionResult ChangePassword()
        {
            var v = new ChangePasswordViewModel();
            if (Factory.CurrentUser.j03IsMustChangePassword)
            {
                this.AddMessage("Administrátor nastavil, že si musíte změnit přihlašovací heslo.", "info");
            }
            return View(v);
        }
        [HttpPost]
        public IActionResult ChangePassword(Models.ChangePasswordViewModel v)
        {            
            var c = new BO.CLS.PasswordChecker();
            var res = c.CheckPassword(v.NewPassword, Factory.App.PasswordMinLength, Factory.App.PasswordMaxLength, Factory.App.PasswordRequireDigit, Factory.App.PasswordRequireUppercase, Factory.App.PasswordRequireLowercase, Factory.App.PasswordRequireNonAlphanumeric);
            if (res.Flag == BO.ResultEnum.Failed)
            {
                this.AddMessage(res.Message); return View(v);
            }
            if (v.NewPassword != v.VerifyPassword)
            {
                this.AddMessage("Heslo nesouhlasí s jeho ověřením."); return View(v);
            }

            var cJ03 = Factory.j03UserBL.Load(Factory.CurrentUser.pid);
            var lu = new BO.LoggingUser();
            
            res=lu.VerifyHash(v.CurrentPassword, cJ03.j03Login, cJ03);            
            if (res.Flag == BO.ResultEnum.Success)
            {
                cJ03.j03PasswordHash = lu.Pwd2Hash(v.NewPassword, cJ03);
                cJ03.j03IsMustChangePassword = false;
                if (Factory.j03UserBL.Save(cJ03) > 0)
                {
                    this.AddMessage("Heslo bylo změněno.", "info");
                    return RedirectToAction("Index");
                }
                             
            }
            else
            {
                this.AddMessage(res.Message);            
            }
            return View(v);

        }


       
        

       
       

    

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode = null)
        {
            var v = new ErrorViewModel() { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier };

            var errFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();
            if (errFeature != null)
            {
                v.Error = errFeature.Error;
            }
            
            
            
            var path = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            if (path != null)
            {
                v.OrigFullPath = path.Path;
            }
            
            

            var statusFeature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            if (statusFeature != null)
            {
                v.OrigFullPath = statusFeature.OriginalPath;
                
            }

            v.OrigFullPath += HttpContext.Request.QueryString;
         
            return View(v);
        }
    }
}
