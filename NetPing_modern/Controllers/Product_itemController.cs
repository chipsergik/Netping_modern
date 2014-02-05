using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NetPing.DAL;

namespace NetPing.Controllers
{
    public class Product_itemController : Controller
    {
        //
        // GET: /Product/

        public ActionResult Index(string id)
        {

            var repository = new SPOnlineRepository();

            var device = repository.Devices.Where(dev => dev.Key.Replace("#", "") == id).FirstOrDefault();

            if (device == null) return View("Error", new Errors("Неверный параметр!"));

            //Create list of connected devices
            ViewBag.Connected_devices = device.Connected_devices.Select(d =>
                                                                           repository.Devices.Where(dv => dv.Name == d).FirstOrDefault()
                                                                      ).ToList();

            ViewBag.Parameter_groups = repository.TermsDeviceParameters.Where(par => par.Level == 0).ToList();

            if (device.Key.Contains("_solut_")) return View("Solutions", device);



            return View(device);
        }

    }
}
