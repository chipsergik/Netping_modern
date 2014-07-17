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
using NetPing.Models;
using NetPing_modern.Models;


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

        public ActionResult Compare(int[] compare)
        {
            var model = new DevicesCompare();

            if (compare == null || compare.Length < 2)
                return View(model);

            var repository = new SPOnlineRepository();
            
            model.Devices = repository.Devices.Where(d => compare.Contains(d.Id)).ToList();
            IEnumerable<DeviceParameter> collection = null;
            var deviceParameterEqualityComparer = new DeviceParameterEqualityComparer();
            for (int i = 0; i < model.Devices.Count - 1; i++)
            {
                var device = model.Devices[i];
                var next = model.Devices[i + 1];
                if (collection == null)
                {
                    collection = device.DeviceParameters.Union(next.DeviceParameters, deviceParameterEqualityComparer);
                }
                else
                {
                    collection = collection.Union(next.DeviceParameters, deviceParameterEqualityComparer);
                }
            }

            if (collection != null)
                model.Parameters = new List<DeviceParameter>(collection.Distinct(deviceParameterEqualityComparer));

            return View(model);
        }
	}
}