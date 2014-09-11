using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NetPing_modern.Controllers
{
    public class BlogController : Controller
    {
        //
        // GET: /Blog/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Main()
        {
            return View();
        }

        public ActionResult Record()
        {
            return View();
        }
	}
}