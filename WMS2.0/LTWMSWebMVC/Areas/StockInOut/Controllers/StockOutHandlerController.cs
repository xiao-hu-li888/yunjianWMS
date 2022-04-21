using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LTWMSWebMVC.Areas.StockInOut.Controllers
{
    public class StockOutHandlerController : BaseController
    {
        // GET: StockInOut/StockOut
        public ActionResult Index()
        {
            return View();
        }
    }
}