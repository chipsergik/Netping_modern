using System.Web.Mvc;
using NetPing.DAL;

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
            return RedirectToAction("Post", "Blog", new {id = id});
        }
    }
}
