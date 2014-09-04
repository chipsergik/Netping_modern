using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NetPing.DAL;
using NetPing_modern.Properties;

namespace NetPing.Controllers
{
    public class MainPageController : Controller
    {
        //
        // GET: /MainPage/

        public ActionResult Index()
        {
            var repository = new SPOnlineRepository();
            var posts = repository.Posts.Where(pst => pst.Cathegory == "News");
            posts = posts.OrderByDescending(item => item.Created);
            ViewBag.posts = posts;
            
            ViewBag.Devices = NetpingHelpers.Helpers.GetNewDevices();


             
            ViewBag.Banners=repository.PubFiles;

            return View();
        }

    }
}
