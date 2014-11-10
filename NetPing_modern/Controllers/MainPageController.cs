﻿using System.Linq;
using System.Web.Mvc;
using NetPing.DAL;

namespace NetPing.Controllers
{
    public class MainPageController : Controller
    {
        private readonly IRepository _repository;

        public MainPageController(IRepository repository)
        {
            _repository = repository;
        }

        //
        // GET: /MainPage/

        public ActionResult Index()
        {
            var posts = _repository.Devices.FirstOrDefault(dev => dev.Name.Level == 0).Posts.Where(pst => pst.Category.Name == "News");
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

            ViewBag.Banners = _repository.PubFiles;

            return View();
        }

    }
}
