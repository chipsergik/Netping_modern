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
            var posts = repository.Devices.FirstOrDefault(dev => dev.Name.Level == 0).Posts.Where(pst => pst.Cathegory == "News");
            posts = posts.OrderByDescending(item => item.Created);
            ViewBag.posts = posts;

            ViewBag.Devices = NetpingHelpers.Helpers.GetNewDevices();


            //Main page banners list
            /*
            var Banners = repository.PubFiles;
            string banner_string = "";
            foreach (var banner in Banners )
            {
                banner_string = banner_string + '"'+banner.Url+'"'+",";
            }
            ViewBag.Banners = banner_string.TrimEnd(',');
             */
            ViewBag.Banners=repository.PubFiles;

            return View();
        }

    }
}
