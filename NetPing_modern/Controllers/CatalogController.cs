using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NetPing.DAL;
/*
    Страница каталога отображает 3 раздела каталога, в каждом из разделов подкаталоги:
        Удалённый мониторинг датчиков - VieBags.id=_nping_bases     /catalog.aspx?id=_nping_bases
            Устройства  ViewBags.sub=_nping_bases                   /catalog.aspx?id=_nping_bases&sub=_nping_bases
            Датчики     ViewBags.sub=_acces_sensr                   /catalog.aspx?id=_nping_bases&sub=_acces_sensr
            Аксессуары  ViewBags.sub=_acces_mains                   /catalog.aspx?id=_nping_bases&sub=_acces_mains
            Готовые решения ViewBags.sub=_solut_sensr               /catalog.aspx?id=_nping_bases&sub=_solut_sensr

        Удалённое управление элекропитанием и перезагрузкой устройств - VieBags.id=_nping_power   /catalog.aspx?id=_nping_power
            Устройства  ViewBags.sub=_nping_power                   /catalog.aspx?id=_nping_power&sub=_nping_power
            Датчики     ViewBags.sub=_acces_sensr                   /catalog.aspx?id=_nping_power&sub=_acces_sensr
            Аксессуары  ViewBags.sub=_acces_mains                   /catalog.aspx?id=_nping_power&sub=_acces_mains
            Готовые решения ViewBags.sub=_solut_sensr               /catalog.aspx?id=_nping_power&sub=_solut_sensr

        Коммутаторы Ethernet POE устройства - VieBags.id=_swtch   /catalog.aspx?id=_swtch
            Устройства  ViewBags.sub=_swtch                         /catalog.aspx?id=_swtch&sub=_swtch
            Аксессуары  ViewBags.sub=_acces_mains                    /catalog.aspx?id=_swtch&sub=_acces_mains

    При переходе от одного раздела или подраздела к другому, страница перезагружается с сервера.

    В зависимости от того какой из разделов активен, он отображается как крайний левый с раскрытыми под разделами

    Активный подраздел нужно выделить визуально

    Серверная часть передаёт на страницу список устройств в зависимости от активного раздела и под раздела.

    !!! Чек-бокс и кнопку сравнения выводить, но сделать неактивными, этот функционал будут добавлен позже. !!!

*/
using NetPing_modern.DAL;


namespace NetPing.Controllers
{
    public class CatalogController : Controller
    {
        //
        // GET: /Catalog/
        public ActionResult Index(string id, string sub)
        {
            string[] id_choice = { CategoryId.MonitoringId, CategoryId.PowerId, CategoryId.SwitchesId };
            var repository = new SPOnlineRepository();

            id=id_choice.FirstOrDefault(x => x == id);
            if (id == null) id = id_choice[0];
            if (sub == "" || sub == null) sub = id;

            var Devices = repository.GetDevices(id, sub);
            ViewBag.id = id;
            ViewBag.sub = sub;
            return View(Devices);
        }
	}
}