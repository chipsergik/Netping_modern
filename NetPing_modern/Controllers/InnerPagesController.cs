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
/*
        public ActionResult Question()
        {
            return View("Question");
        }
*/
        public ActionResult UCache()
        {
            ViewBag.result = _repository.UpdateAll();

            return View("UCache");
        }

        public ActionResult Buy(string id)
        {
            switch (id)
            {
                case "":
                case null:
                    ViewBag.Text = _repository.SiteTexts.FirstOrDefault(t => t.Tag == "Buy").Text;
                    ViewBag.Head = "Купить устройства Netping";
                    ViewBag.Title = "Купить устройства Netping";
                    ViewBag.Description = "Как купить устройства Netping";
                    ViewBag.Keywords = "netping купить";
                    break;
                case "dealers":
                    ViewBag.Text = _repository.SiteTexts.FirstOrDefault(t => t.Tag == "Dealers").Text;
                    ViewBag.Head = "Наши партнёры";
                    ViewBag.Title = "Netping дилеры";
                    ViewBag.Description = "список дилеров, партнёров компании Netping";
                    ViewBag.Keywords = "netping дилеры, netping партнёры";
                    break;
                case "partnership-how-to":
                    ViewBag.Text = _repository.SiteTexts.FirstOrDefault(t => t.Tag == "Partnership-how").Text;
                    ViewBag.Head = "Как стать нашим партнёром?";
                    ViewBag.Title = "Как стать партнёром компании Netping?";
                    ViewBag.Description = "Как стать партнёром, дилером компании Netping";
                    ViewBag.Keywords = "как стать дилером Netping, как стать партнёром Netping";
                    break;

                default:
                    return HttpNotFound();
            }

            return View("InnerPage");
        }

        public ActionResult About()
        {
            var text = _repository.SiteTexts.FirstOrDefault(t => t.Tag=="About").Text;
//            if (text == null) return HttpNotFound();

            ViewBag.Head = "О компании Netping";
            ViewBag.Text = text;
            ViewBag.Title = "О компании Netping";
            ViewBag.Description = "О компании Netping, история компании и основные достижения";
            ViewBag.Keywords = "netping, о компании";

            return View("InnerPage");
        }

        public ActionResult Contacts()
        {
            var text = _repository.SiteTexts.FirstOrDefault(t => t.Tag=="Contact_us").Text;
//            if (text == null) return HttpNotFound();

            ViewBag.Head = "Контакты компании Netping";
            ViewBag.Text = text;
            ViewBag.Title = "Контакты компании Netping";
            ViewBag.Description = "адрес, телефон, банковские реквизиты компании Netping";
            ViewBag.Keywords = "контакты компании, банковские реквизиты, адрес офиса";

            return View("InnerPage");
        }

        public ActionResult Vacancy()
        {
            var text = _repository.SiteTexts.FirstOrDefault(t => t.Tag == "Vacancy").Text;
//            if (text == null) return HttpNotFound();

            ViewBag.Head = "Мы ищем сотрудников!";
            ViewBag.Text = text;
            ViewBag.Title = "Вакансии компании Netping";
            ViewBag.Description = "список вакансий компании Netping";
            ViewBag.Keywords = "вакансии";

            return View("InnerPage");
        }

        public ActionResult Support()
        {
            var text = _repository.SiteTexts.FirstOrDefault(t => t.Tag == "Support").Text;
//            if (text == null) return HttpNotFound();

            ViewBag.Head = "Техническая поддержка устройств Netping";
            ViewBag.Text = text;
            ViewBag.Title = "Техническая поддержка устройств Netping";
            ViewBag.Description = "как получить техническую поддержку по устройствам Netping";
            ViewBag.Keywords = "техническая поддержка, саппорт";

            return View("InnerPage");
        }

        public ActionResult Dev()
        {
            return View();
        }


    }
}
