﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BTI_Project1_API.Controllers
{
    public class ReadmeController : Controller
    {
        // GET: ReadmeController
        public ActionResult Index()
        {
            return View();
        }
    }
}
