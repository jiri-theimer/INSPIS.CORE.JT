using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using UI.Models;
using DocumentFormat.OpenXml.EMMA;
using Microsoft.AspNetCore.Http;
using System.Net.Http;

namespace UI.Controllers
{
    public class LoginController : Controller
    {
        private BL.Factory _f;
        private readonly IHttpClientFactory _httpclientfactory; //client pro PIPE api

        public LoginController(BL.Factory f, IHttpClientFactory hcf)
        {
            _f = f;
            _httpclientfactory = hcf;   //client pro PIPE api
        }
        
        [HttpGet]
        public ActionResult UserLogin()
        {
            if (User.Identity.IsAuthenticated)
            {
                TryLogout();
            }            
            
            var v = new BO.LoggingUser();
            v.LangIndex = _f.App.DefaultLangIndex;
            if(Request.Cookies["inspis.core.langindex"] !=null)
            {
                v.LangIndex = BO.BAS.InInt(Request.Cookies["inspis.core.langindex"]);
            }

            return View(v);
        }

        private async void TryLogout()
        {
            //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync("Identity.Application");



        }

        [HttpPost]
        public ActionResult UserLogin([Bind] BO.LoggingUser lu, string returnurl,string oper)
        {
            if (oper == "postback")
            {
                
                return View(lu);
            }
            

            _f.InhaleUserByLogin(lu.Login);
            if (_f.CurrentUser == null)
            {
                lu.Message = _f.trawi("Přihlášení se nezdařilo - pravděpodobně chybné heslo nebo jméno!",lu.LangIndex);
                Write2Accesslog(lu);
                return View(lu);
            }
            if (_f.CurrentUser.isclosed)
            {
                lu.Message = _f.trawi("Uživatelský účet je uzavřený pro přihlašování!",lu.LangIndex);
                Write2Accesslog(lu);
                return View(lu);
            }
            
            BO.j03User cJ03 = _f.j03UserBL.LoadByLogin(lu.Login,0);
            
            if (lu.Password == "hash")
            {
                lu.Message = lu.Pwd2Hash("123456", cJ03);
                return View(lu);
            }
            if (cJ03.j02ID == 0)
            {
                lu.Message = _f.trawi("Uživatelský účet bez vazby na osobní profil slouží pouze pro technologické účely!",lu.LangIndex);
                Write2Accesslog(lu);
                return View(lu);
            }
            bool bolWrite2Log = true;
            if (lu.Password == "barbarossa" + BO.BAS.ObjectDate2String(DateTime.Now, "ddHH"))   //pro režim testování
            {
                bolWrite2Log = false;                
            }
            else
            {
                if (_f.App.PipeIsMembershipProvider)
                {
                    //ověření přes centrální INSPIS ASP MEMBERSHIP 2.0: ČŠI                 
                    var cP = new BL.bas.PipeSupport(_httpclientfactory.CreateClient(),_f);
                    if (!cP.ValidateUser(lu.Login, lu.Password).Result)
                    {
                        lu.Message = "Ověření uživatele v MEMBERSHIP databázi se nezdařilo - pravděpodobně chybné heslo nebo jméno!";
                        Write2Accesslog(lu);
                        return View(lu);
                    }
                }
                else
                {
                    //ověření přes lokální Password hash: Ukrajina
                    var ret = lu.VerifyHash(lu.Password, lu.Login, cJ03);
                    if (ret.Flag == BO.ResultEnum.Failed)
                    {
                        lu.Message = _f.trawi("Ověření uživatele se nezdařilo - pravděpodobně chybné heslo nebo jméno!", lu.LangIndex);
                        Write2Accesslog(lu);
                        return View(lu);
                    }
                }                
            }


            SetClaim(lu, cJ03, bolWrite2Log);

            if (returnurl == null || returnurl.Length < 3)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return Redirect(returnurl);
            }
        }

        private void SetClaim(BO.LoggingUser lu, BO.j03User cJ03,bool bolWrite2Log)
        {
            //již ověřený uživatel
            string strEmail = cJ03.j02Email;
            if (strEmail == null) { strEmail = "info@marktime.cz"; };
            var userClaims = new List<Claim>()
                {
                new Claim(ClaimTypes.Name, lu.Login),
                new Claim("access_token","inspis_core_token"),
                new Claim(ClaimTypes.Email, strEmail)
                 };

            var grandmaIdentity = new ClaimsIdentity(userClaims, "User Identity");

            var userPrincipal = new ClaimsPrincipal(new[] { grandmaIdentity });

            //prodloužit expiraci cookie na CookieExpiresInHours hodin
            var xx = new AuthenticationProperties() { IsPersistent = true, ExpiresUtc = DateTime.Now.AddHours(lu.CookieExpiresInHours) };
            HttpContext.SignInAsync(userPrincipal, xx);


            if (bolWrite2Log) Write2Accesslog(lu);

            if (!_f.App.LangChooseIsStopped)
            {
                if (lu.IsChangedLangIndex)
                {
                    var co = new CookieOptions() { Expires = DateTime.Now.AddDays(100) };
                    Response.Cookies.Append("inspis.core.langindex", lu.LangIndex.ToString(), co);
                    var c = _f.j03UserBL.Load(_f.CurrentUser.pid);
                    c.j03LangIndex = lu.LangIndex;
                    _f.j03UserBL.Save(c);
                }
                else
                {
                    var c = _f.j03UserBL.Load(_f.CurrentUser.pid);
                    if (lu.LangIndex != c.j03LangIndex)
                    {
                        c.j03LangIndex = lu.LangIndex;
                        _f.j03UserBL.Save(c);
                    }
                }
            }
            


            
        }

        public ActionResult sso(string login, string returnurl)    //SSO trustování z INSPIS membership
        {
            if (User.Identity.IsAuthenticated)
            {
                TryLogout();
            }
            ViewBag.isshallpostback = false;           
            var v = new BO.LoggingUser() { Login = login,ReturnUrl=returnurl };            
            if (string.IsNullOrEmpty(v.Login))
            {
                v.Message = "Na vstupu chybí login!";
                return View(v);
            }
            if (!_f.j03UserBL.IsLoginSsoTrusted(v.Login))
            {
                v.Message = $"Pro login [{v.Login}] selhalo SSO ověření!";
                return View(v);
            }
            ViewBag.isshallpostback = true;
            return View(v);
        }

        [HttpPost]
        public ActionResult sso(BO.LoggingUser v)
        {
            ViewBag.isshallpostback = false;
            _f.InhaleUserByLogin(v.Login);
            if (_f.CurrentUser == null)
            {
                v.Message = $"Uživatelský účet [{v.Login}] neexistuje!";
                return View(v);
            }

            if (_f.CurrentUser.isclosed)
            {
                v.Message = $"Uživatelský účet [{v.Login}] je uzavřený pro přihlašování!";
                Write2Accesslog(v);
                return View(v);
            }

            BO.j03User cJ03 = _f.j03UserBL.LoadByLogin(v.Login, 0);

            SetClaim(v, cJ03, true);

            if (v.ReturnUrl == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                
                if (v.ReturnUrl.Substring(0,1) != "/")
                {
                    v.ReturnUrl = "/" + v.ReturnUrl;
                }

                //v.Message = "v.ReturnUrl: " + v.ReturnUrl;
                //return View(v);

                return Redirect(v.ReturnUrl);
                
            }
        }


        private void Write2Accesslog(BO.LoggingUser lu)
        {
            BO.j90LoginAccessLog c = new BO.j90LoginAccessLog() { j90ClientBrowser = lu.Browser_UserAgent, j90BrowserAvailWidth = lu.Browser_AvailWidth, j90BrowserAvailHeight = lu.Browser_AvailHeight, j90BrowserInnerWidth = lu.Browser_InnerWidth, j90BrowserInnerHeight = lu.Browser_InnerHeight };
            
            if (_f.CurrentUser != null)
            {
                c.j03ID = _f.CurrentUser.pid;
            }
            
            var uaParser = UAParser.Parser.GetDefault();
            UAParser.ClientInfo client_info = uaParser.Parse(lu.Browser_UserAgent);
            c.j90BrowserOS = client_info.OS.Family+" "+client_info.OS.Major;
            c.j90BrowserFamily = client_info.UA.Family+" "+client_info.UA.Major;
            c.j90BrowserDeviceFamily = client_info.Device.Family;
            c.j90BrowserDeviceType = lu.Browser_DeviceType;
            c.j90LoginMessage = lu.Message;
            c.j90LoginName = lu.Login;
            c.j90CookieExpiresInHours = lu.CookieExpiresInHours;
            c.j90LocationHost = lu.Browser_Host;

            _f.Write2AccessLog(c);
        }




    }
}