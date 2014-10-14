using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
            var posts = _repository.Posts;
            var pst = posts.Where(item => item.Id == id).FirstOrDefault();
            if (pst == null) return View("Error", new Errors("Неверный параметр!"));
            return View(pst);
        }
    }
}
