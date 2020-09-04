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
    public class aloginController : Controller
    {
        public async Task<ActionResult> Index()
        {
            await HttpContext.SignOutAsync();
            return View();
        }

        public async Task<ActionResult> Login(string login)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, login),
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTime.Now.AddDays(15),
                IsPersistent = false
            };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            return View("OK");
        }
    }
}
