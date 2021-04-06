﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Controllers
{
    public class PingController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Time = DateTime.Now.ToString();
            return View();
        }
    }
}
