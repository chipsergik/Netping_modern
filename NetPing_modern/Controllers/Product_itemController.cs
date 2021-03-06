﻿using System.Linq;
using System.Web.Mvc;
using NetPing.DAL;

namespace NetPing.Controllers
{
    public class Product_itemController : Controller
    {
        private readonly IRepository _repository;

        public Product_itemController(IRepository repository)
        {
            _repository = repository;
        }

        //
        // GET: /Product/

        public ActionResult Index(string id)
        {
            var device = _repository.Devices.Where(dev => dev.OldKey == id).FirstOrDefault();

            if (device == null) return RedirectPermanent("/products");  // if key incorrect go to /products

            return RedirectPermanent("/products/" + device.Url);
        }

        public ActionResult Development()
        {
            return View();
        }
    }
}
