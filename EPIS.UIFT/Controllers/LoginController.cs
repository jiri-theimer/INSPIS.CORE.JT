using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace UIFT.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly UIFT.Repository.RepositoryFactory Factory;

        public LoginController(UIFT.Repository.RepositoryFactory factory)
        {
            this.Factory = factory;
        }

        [HttpGet]
        public async Task<ActionResult> Index(string pin = "", string akce = "")
        {
            Models.LoginModel model = new Models.LoginModel
            {
                Akce = akce,
                PIN = pin
            };

            // automaticke prihlaseni
            if (!string.IsNullOrEmpty(pin) && !string.IsNullOrEmpty(akce))
            {
                return await Login(model);
            }

            return View("Index", model);
        }

        /// <summary>
        /// Odhlaseni
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return View("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(Models.LoginModel model)
        {
            var rep = Factory.Get();

            if (string.IsNullOrEmpty(model.PIN))
            {
                ModelState.AddModelError("", rep.BL.tra("Musíte zadat PIN."));
            }
            else if (string.IsNullOrEmpty(model.Akce))
            {
                ModelState.AddModelError("", rep.BL.tra("Musíte zadat ID akce."));
            }
            else
            {
                try
                {
                    // informace o akci
                    var model2 = rep.BL.a11EventFormBL.LoadPoll(model.Akce, model.PIN);
                    
                    if (model2 != null)
                    {
                        if (!model2.isclosed && !model2.a01IsClosed)
                        {
                            // prihlasit umeleho uzivatele
                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, rep.BL.GlobalParams.LoadParam("UIFT_AnonymousUser")),
                                new Claim("PollIdAkce", model.Akce),
                                new Claim("PollPin", model.PIN),
                                new Claim("a11ID", model2.pid.ToString())
                            };
                            var claimsIdentity = new ClaimsIdentity(claims, "Identity.Application");

                            var authProperties = new AuthenticationProperties
                            {
                                ExpiresUtc = DateTime.Now.AddHours(4),
                                IsPersistent = false
                            };
                            await HttpContext.SignInAsync("Identity.Application", new ClaimsPrincipal(claimsIdentity), authProperties);
                        
                            // presmerovat na formular
                            return RedirectToRoute("form", new { a11id = model2.pid });
                        }
                    }

                    ModelState.AddModelError("", rep.BL.tra("Pro zadaný PIN a ID akce systém nedokázal najít otevřenou anketu."));
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", rep.BL.tra("Chybné přístupové údaje."));
                }
            }

            return View("Index");
        }

        /// <summary>
        /// Dialog po zamkceni ankety
        /// </summary>
        [HttpGet]
        public ActionResult AnketaZamkcena()
        {
            return View();
        }
    }
}
