using System.Web.Mvc;
using NetPing.DAL;
using System.Collections.Generic;
using System.Linq;


namespace NetPing.Controllers
{
    public class ViewController : Controller
    {
        private readonly IRepository _repository;

        public ViewController(IRepository repository)
        {
            _repository = repository;
        }

        //
        // GET: /View/



        public ActionResult Index(int id)
        {
            
            var post=_repository.Posts.FirstOrDefault(p => p.Id == id);
            if (post == null) return RedirectPermanent("Blog/no-found-article");
            return RedirectPermanent(post.Url_name);
        }
 
    }
}
