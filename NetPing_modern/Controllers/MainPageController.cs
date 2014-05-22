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
            
            // var banners = repository.pubfiles;
           
            //string banner_string = "";
            //foreach (var banner in banners )
            //{
            //    banner_string = banner_string + '"'+banner.url+'"'+",";
            //}
            //viewbag.banners = banner_string.trimend(',');
             
            ViewBag.Banners=repository.PubFiles;

            return View();
        }

    }
}
