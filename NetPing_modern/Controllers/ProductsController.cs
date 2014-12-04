using System.Linq;
using System.Web.Mvc;
using NetPing.DAL;
using NetPing_modern.Helpers;
using NetPing_modern.Models;
using WebGrease.Css.Extensions;

namespace NetPing_modern.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IRepository _repository;

        public ProductsController(IRepository repository)
        {
            _repository = repository;
        }


        public ActionResult Device_view(string id)
        {

            var device = _repository.Devices.Where(dev => dev.Url == id).FirstOrDefault();

            if (device == null) return Redirect("/products");  // if key incorrect go to /products

            //Create list of connected devices
            var connected_devices = device.Connected_devices.Select(d => _repository.Devices.Where(dv => dv.Name == d).FirstOrDefault()).ToList();
            ViewBag.Connected_devices_accessuars = connected_devices.Where(d => d != null && !d.Name.Path.Contains("Sensors")).ToList();
            ViewBag.Connected_devices_sensors = connected_devices.Where(d => d != null && d.Name.Path.Contains("Sensors")).ToList();

            ViewBag.Parameter_groups = _repository.TermsDeviceParameters.Where(par => par.Level == 0).ToList();
            ViewBag.Files_groups = _repository.TermsFileTypes.Where(type => type.Level == 0).ToList();

            //Device group
            var dev_path = device.Name.Path.Split(';');
            var grp = dev_path[dev_path.Length - 2];
            var group_dev = _repository.Devices.FirstOrDefault(dev => dev.Name.OwnNameFromPath == grp);
            ViewBag.grp_name = group_dev.Name.Name;
            ViewBag.grp_url = Url.Action("Index", "Products", new { group = group_dev.Url });


            return View("Device_view",device);
        }

        public ActionResult Index(string group, string id)
        {
            var devices = _repository.Devices.Where(d => !d.Name.IsGroup());
            var g = _repository.Devices.FirstOrDefault(d => d.Url == @group);
            if (g != null)
            {
                if (!g.Name.IsGroup()) return Device_view(group);  // Open device page
                devices = devices.Where(d => !d.Name.IsGroup() && d.Name.IsUnderOther(g.Name));
            }
            else { return HttpNotFound(); }

            var model = new ProductsModel
                        {
                            ActiveSection =
                                NavigationProvider.GetAllSections().First(m => m.Url == @group)
                        };
            

            if (!string.IsNullOrEmpty(id))
            {
                var sub = _repository.Devices.FirstOrDefault(d => d.Url == id);
                if (sub != null)
                {
                    devices = _repository.Devices.Where(d => !d.Name.IsGroup() && d.Name.IsUnderOther(sub.Name));
                }
                else { return HttpNotFound(); }
                model.ActiveSection.Sections.First(m => m.Url  == id).IsSelected = true;
            }
            else
            {
                model.ActiveSection.Sections.First().IsSelected = true;
            }

            model.Devices = devices;
            model.ActiveSection.IsSelected = true;
            var sections = NavigationProvider.GetAllSections().Where(m => m.Url != model.ActiveSection.Url);
            sections.ForEach(m => model.Sections.Add(m));
            return View(model);
        }
    }
}