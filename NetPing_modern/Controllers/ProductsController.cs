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

        public ActionResult Index(string group, string id)
        {
            var devices = _repository.Devices.Where(d => !d.Name.IsGroup());
            var g = _repository.Devices.FirstOrDefault(d => d.Url == @group);
            if (g != null)
            {
                devices = devices.Where(d => !d.Name.IsGroup() && d.Name.IsUnderOther(g.Name));
            }

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
                model.ActiveSection.Sections.First(m => m.Url == id).IsSelected = true;
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