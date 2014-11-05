using System.Linq;
using System.Web.Mvc;
using NetPing.DAL;
using NetPing.Models;
using NetPing_modern.Mappers;
using NetPing_modern.ViewModels;

namespace NetPing_modern.Controllers
{
    public class BlogController : Controller
    {
        private readonly IRepository _repository;
        private readonly IMapper<Post, PostViewModel> _postMapper;

        public BlogController(IRepository repository, IMapper<Post,PostViewModel> postMapper)
        {
            _repository = repository;
            _postMapper = postMapper;
        }

        public ActionResult Main()
        {
            var model = new BlogViewModel();

            var posts = _repository.Devices.FirstOrDefault(dev => dev.Name.Level == 0).Posts.Where(pst => pst.Cathegory == "News");
            model.Posts = posts.OrderByDescending(item => item.Created).Select(_postMapper.Map).Take(100);
            return View(model);
        }

        public ActionResult Record(string postname)
        {
            var posts = _repository.Posts;
            var pst = posts.FirstOrDefault(item => item.Url_name == ("/Blog/"+postname));
            if (pst == null) return View("Error", new Errors("Такой записи в блоге не существует!"));
            return View(pst);
        }
	}
}