using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NetPing.DAL;

namespace NetPing.Controllers
{
    public class CatalogController : Controller
    {
        //
        // GET: /Catalog/
        public ActionResult Index(string id, string sub)
        {
            string[] id_choice = { "_nping_bases", "_nping_power", "_swtch" };
            var repository = new SPOnlineRepository();

            id=id_choice.FirstOrDefault(x => x == id);
            if (id == null) id = id_choice[0];
            if (sub == "" || sub == null) sub = id;

            var Devices = repository.Devices.Where(dev => dev.Key.Contains(sub) 
                                                   && 
                                                   !dev.Key.Contains("#")
                                                   &&
                                                   dev.Label.OwnNameFromPath != "Archive"
                                                  );

            ViewBag.id = id;
            ViewBag.sub = sub;
            return View(Devices);
        }
	}
}