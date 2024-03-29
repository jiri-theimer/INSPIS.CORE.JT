﻿using Microsoft.AspNetCore.Authentication;
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
        private readonly int LangIndex;

        public LoginController(UIFT.Repository.RepositoryFactory factory, BL.RunningApp app)
        {
            this.LangIndex = app.DefaultLangIndex;
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

            try
            {
                BO.RunningUser ru = (BO.RunningUser)HttpContext.RequestServices.GetService(typeof(BO.RunningUser));
                if (string.IsNullOrEmpty(ru.j03Login))
                {
                    ru.j03Login = HttpContext.User.Identity.Name;
                }
                model.PrihlasenyUzivatel = ru.j03Login;
            }
            catch
            {
                model.PrihlasenyUzivatel = "??";
            }
            

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
            
            bool bolAnonym = false;            
            BO.RunningUser ru = (BO.RunningUser)HttpContext.RequestServices.GetService(typeof(BO.RunningUser));
            if (string.IsNullOrEmpty(ru.j03Login))
            {
                ru.j03Login = HttpContext.User.Identity.Name;                
            }
            if (string.IsNullOrEmpty(ru.j03Login) || ru.j03Login.ToLower()=="anketa")
            {
                bolAnonym = true;
            }

            var rep = Factory.Get();

            if (string.IsNullOrEmpty(model.PIN))
            {
                ModelState.AddModelError("", rep.BL.trawi("Musíte zadat PIN.", this.LangIndex));
            }
            else if (string.IsNullOrEmpty(model.Akce))
            {
                ModelState.AddModelError("", rep.BL.trawi("Musíte zadat ID akce.", this.LangIndex));
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
                            //if (string.IsNullOrEmpty(model.PrihlasenyUzivatel))
                            if (bolAnonym)
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
                            }


                            // presmerovat na formular
                            return RedirectToRoute("form", new { a11id = model2.pid });
                        }
                    }

                    ModelState.AddModelError("", rep.BL.trawi("Pro zadaný PIN a ID akce systém nedokázal najít otevřenou anketu.", this.LangIndex));
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", rep.BL.trawi("Chybné přístupové údaje.", this.LangIndex));
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
