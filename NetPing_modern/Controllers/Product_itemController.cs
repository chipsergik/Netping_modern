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

            //if (device == null) return View("Error", new Errors("Неверный параметр!"));
            if (device == null) return Redirect("/catalog.aspx");  // if key incorrect go to /catalog.aspx


            //Create list of connected devices
            var connected_devices = device.Connected_devices.Select(d =>
                                                                           repository.Devices.Where(dv => dv.Name == d).FirstOrDefault()
                                                                      ).ToList();
            ViewBag.Connected_devices_accessuars = connected_devices.Where(d => d!=null && !d.Name.Path.Contains("Sensors")).ToList();
            ViewBag.Connected_devices_sensors = connected_devices.Where(d => d!=null &&  d.Name.Path.Contains("Sensors")).ToList();

            ViewBag.Parameter_groups = repository.TermsDeviceParameters.Where(par => par.Level == 0).ToList();

            //Device group
            var dev_path = device.Name.Path.Split(';');
            var grp = dev_path[dev_path.Length - 2];
            var group_dev=repository.Devices.FirstOrDefault(dev => dev.Name.OwnNameFromPath == grp);
            ViewBag.grp_name = group_dev.Name.Name;
            ViewBag.grp_url = group_dev.GroupUrl;

            //if (device.Key.Contains("_solut_")) return View("Solutions", device);

            return View(device);
        }

    }
}
