using System.Linq;
using System.Web.Mvc;
using NetPing.DAL;
using NetPing_modern.Models;

namespace NetPing_modern.Controllers
{
    public class BlogController : Controller
    {
        private readonly IRepository _repository;

        public BlogController(IRepository repository)
        {
            _repository = repository;
        }

        public ActionResult Main()
        {
            var model = new Blog();

            var posts = _repository.Devices.FirstOrDefault(dev => dev.Name.Level == 0).Posts.Where(pst => pst.Cathegory == "News");
            model.Posts = posts.OrderByDescending(item => item.Created).Take(100);
            return View(model);
        }

        public ActionResult Record(string postname)
        {
            var posts = _repository.Posts;
            var pst = posts.FirstOrDefault(item => item.Url_name == postname);
            if (pst == null) return View("Error", new Errors("Такой записи в блоге не существует!"));
            return View(pst);
        }
	}
}