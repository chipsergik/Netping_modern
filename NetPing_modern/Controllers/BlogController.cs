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

            var posts = _repository.Devices.FirstOrDefault(dev => dev.Name.Level == 0).Posts.ToList();
            model.Posts = posts.OrderByDescending(item => item.Created).Select(_postMapper.Map);
            return View(model);
        }

        public ActionResult Record(string postname)
        {
            var model = new BlogViewModel();
            var posts = _repository.Posts;
            model.Post = _postMapper.Map(posts.FirstOrDefault(item => item.Url_name == string.Format("/Blog/{0}", postname)));
            model.Posts =
                _repository.Devices.FirstOrDefault(dev => dev.Name.Level == 0)
                    .Posts.OrderByDescending(item => item.Created)
                    .Select(_postMapper.Map);
            return View(model);
        }
	}
}