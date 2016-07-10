using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UrlsAndRoutes.Controllers
{
    public class ExampleController : Controller
    {
        // GET: Example
        public ActionResult Index()
        {
            ViewBag.Message = "Hello";
            ViewBag.Date = DateTime.Now;

            ViewData["Message"] = "Hello, again";
            ViewData["Date"] = DateTime.Now;
                                    //ref: this is ViewData.Model
            return View("Homepage", (object)"Hello, World");
        }
    }
}