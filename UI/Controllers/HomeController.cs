﻿using System;
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
using System.Net.Http;

namespace UI.Controllers
{

    

    public class HomeController : BaseController
    {
       
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpclientfactory; //client pro PIPE api

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory hcf)
        {
            _logger = logger;
            _httpclientfactory = hcf;   //client pro PIPE api
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
        public BO.Result SaveCurrentUserFontSize(int fontsize)
        {
            var c = Factory.j03UserBL.Load(Factory.CurrentUser.pid);
            c.j03FontStyleFlag = fontsize;
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
            
            if (HttpContext.Request.Path.Value.Length <= 1)
            {
                //úvodní spuštění: otestovat nastavení domovské stránky
                if (Factory.CurrentUser.j03HomePageUrl != null)
                {
                                        
                    return Redirect(Factory.CurrentUser.getHomePageUrl());  //pryč na jinou stránku
                }
            }

            return RedirectToAction("Widgets", "Dashboard");
           
            

        }

        

        


        public IActionResult About()
        {
            var v = new BaseViewModel();
            return View(v);
        }
        public IActionResult Anketa()
        {
            var v = new BaseViewModel();
            return View(v);
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
            v.Mobile = v.RecJ02.j02Mobile;
            v.Phone = v.RecJ02.j02Phone;
            if (v.RecJ02.a05ID > 0)
            {
                v.a05Name = Factory.a05RegionBL.Load(v.RecJ02.a05ID).a05Name;
            }
            

            return View(v);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MyProfile(Models.MyProfileViewModel v,string oper)
        {
            if (oper == "clearparams")
            {
                Factory.CBL.TruncateUserParams(0);
                this.AddMessage("Server cache vyčištěna.", "info");
                return MyProfile();
            }
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(v.EmailAddres) == true)
                {
                    this.AddMessage("Chybí e-mail adresa.");
                    return MyProfile();
                }
                BO.j02Person c = Factory.j02PersonBL.Load(Factory.CurrentUser.j02ID);
                c.j02Email = v.EmailAddres;
                c.j02Mobile = v.Mobile;
                c.j02Phone = v.Phone;
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
            var cPwdSupp = new BL.bas.PasswordSupport();
            var res = cPwdSupp.CheckPassword(v.NewPassword);
            if (res.Flag == BO.ResultEnum.Failed)
            {
                this.AddMessage(res.Message); return View(v);
            }
            if (v.NewPassword != v.VerifyPassword)
            {
                this.AddMessage("Heslo nesouhlasí s jeho ověřením."); return View(v);
            }

            var cJ03 = Factory.j03UserBL.Load(Factory.CurrentUser.pid);

            if (Factory.App.PipeIsMembershipProvider)
            {
                //heslo v centrální MEMBERSHIP databázi
                var cP = new BL.bas.PipeSupport(_httpclientfactory.CreateClient(), this.Factory);
                if (!cP.ValidateUser(cJ03.j03Login, v.CurrentPassword).Result)
                {
                    this.AddMessageTranslated("Současné heslo není zadané správně.");
                    return View(v);
                }
                var strNewPassword = cP.RecoveryPassword(cJ03.j03Login, v.NewPassword).Result;
                if (strNewPassword == v.NewPassword)
                {
                    this.AddMessageTranslated("Heslo bylo změněno a uloženo do MEMBERSHIP databáze.", "info");
                    return RedirectToAction("Index");
                }
                else
                {
                    this.AddMessageTranslated("Chyba na straně volání PIPE api");
                    return View(v);
                }
            }
            else
            {
                //heslo v aplikační db
                
                res = cPwdSupp.VerifyUserPassword(v.CurrentPassword, cJ03.j03Login, cJ03);
                if (res.Flag == BO.ResultEnum.Success)
                {
                    cJ03.j03PasswordHash = cPwdSupp.GetPasswordHash(v.NewPassword, cJ03);
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
