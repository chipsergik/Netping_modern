using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NetPing.DAL;
using NetPing.Models;
using NetPing_modern.Properties;

namespace NetPing.Controllers
{
    public class InnerPagesController : Controller
    {
        //
        // GET: /InnerPages/

        private IRepository _repository;

        public InnerPagesController(IRepository repository)
        {
            _repository = repository;
        }

        public ActionResult Question()
        {
            return View("Question");
        }

        public ActionResult UCache()
        {
            ViewBag.result = _repository.UpdateAll();

            return View("UCache");
        }

        public ActionResult Dealers()
        {
            var dealers = _repository.SiteTexts.FirstOrDefault(t => t.Tag=="Dealers" );

            return View("Dealers",dealers);
        }

        public ActionResult About()
        {
            var text_about_obj = _repository.SiteTexts.FirstOrDefault(t => t.Tag=="About");
            if (text_about_obj == null) return View("Error", new Errors("Text about not found!"));
            ViewBag.Text=text_about_obj.Text;
            ViewBag.Head = text_about_obj.Tag;

            return View("About");
        }

        public ActionResult Contacts()
        {
            var text_contacts_obj = _repository.SiteTexts.FirstOrDefault(t => t.Tag=="Contact_us");
            if (text_contacts_obj == null) return View("Error", new Errors("Contacts not found!"));
            ViewBag.Contacts = text_contacts_obj;

            var text_bankDetails_obj = _repository.SiteTexts.FirstOrDefault(t => t.Tag=="Payment_methods");
            if (text_bankDetails_obj == null) return View("Error", new Errors("Bank details not found!"));
            ViewBag.BankDetails = text_bankDetails_obj;

            var text_officeAddress_obj = _repository.SiteTexts.FirstOrDefault(t => t.Tag=="Location");
            if (text_officeAddress_obj == null) return View("Error", new Errors("Office address not found!"));
            ViewBag.OfficeAddress = text_officeAddress_obj;

            return View("Contacts");
        }

    }
}
