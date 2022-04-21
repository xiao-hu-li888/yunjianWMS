using LTWMSWebMVC.Areas.ErrorPage.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LTWMSWebMVC.Areas.ErrorPage.Controllers
{
    public class ErrorController : Controller
    {
        // GET: ErrorPage/Error
        public ActionResult Index()
        {
            ErrorModel err = new ErrorModel();
            err.Message = "";
            return View(err);
        }
        [AllowAnonymous]
        public ActionResult Unauthorized()
        {
            return View();
        }
    }
}