using System.Linq;
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
            var device = _repository.Devices.Where(dev => dev.Url == id).FirstOrDefault();

            //if (device == null) return View("Error", new Errors("Неверный параметр!"));
            if (device == null) return Redirect("/products");  // if key incorrect go to /products


            //Create list of connected devices
            var connected_devices = device.Connected_devices.Select(d =>
                                                                           _repository.Devices.Where(dv => dv.Name == d).FirstOrDefault()
                                                                      ).ToList();
            ViewBag.Connected_devices_accessuars = connected_devices.Where(d => d!=null && !d.Name.Path.Contains("Sensors")).ToList();
            ViewBag.Connected_devices_sensors = connected_devices.Where(d => d!=null &&  d.Name.Path.Contains("Sensors")).ToList();

            ViewBag.Parameter_groups = _repository.TermsDeviceParameters.Where(par => par.Level == 0).ToList();
            ViewBag.Files_groups = _repository.TermsFileTypes.Where(type => type.Level == 0).ToList();

            //Device group
            var dev_path = device.Name.Path.Split(';');
            var grp = dev_path[dev_path.Length - 2];
            var group_dev = _repository.Devices.FirstOrDefault(dev => dev.Name.OwnNameFromPath == grp);
            ViewBag.grp_name = group_dev.Name.Name;
            ViewBag.grp_url = Url.Action("Index", "Products", new {group = group_dev.Url});

            //if (device.Key.Contains("_solut_")) return View("Solutions", device);

            return View(device);
        }

    }
}
