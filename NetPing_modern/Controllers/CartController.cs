using NetPing_modern.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace NetPing_modern.Controllers
{
    public class CartController : Controller
    {
        //
        // GET: /Cart/
        public ActionResult Index()
        {
            return View();
        }

        public Boolean? SendCartMail(Cart cart)
        {
            try
            {
                using (var wb = new WebClient())
                {
                    var data = new NameValueCollection();
                    data["authuser"] = "sp@netping.ru";
                    data["authpass"] = "NqIx8Fov";
                    data["from"] = cart.EMail;
                    data["subject"] = "Заказ";
                    data["to"] = "sales@netping.ru";
                    data["content"] = cart.Name + cart.Shipping + cart.Requisites + cart.Address;
                    string items = "";
                    var sum = 0;
                    foreach (var cartitem in cart.Data)
                    {
                        var itemrow = "";
                        var itemprice = Int32.Parse(((string)cartitem["price"]));
                        var itemcount = Int32.Parse(((string)cartitem["count"]));
                        itemrow = String.Format(@"<tr><td><img src='{0}' width=200/></td>
                        <td>{1}</td>
                        <td>{2} руб.</td>
                        <td>{3}</td>
                        <td>{4} руб.</td></tr>",
                                cartitem["photoURL"], cartitem["name"], cartitem["price"], cartitem["count"], itemprice * itemcount);
                        items += itemrow;
                        sum += itemprice * itemcount;
                    }

                    data["html_content"] = String.Format(@"<html><h2>{0}</h2>
                        <h2>Адрес доставки: {1}</h2>
                        <h2>Способ доставки: {2}</h2>
                        <h2>Сумма заказа: {3} руб.</h2>
                        <h2>Состав заказа</h2>
                        <table border=1 style='border: 1px solid #000; width: 100%; border-collapse: collapse'>
                        <tr><th></th><th>Наименование</th><th>Цена</th><th>Количество</th><th>Стоимость</th></tr>
                        {4}</table>
                        </html>", cart.Name, cart.Address, cart.Shipping, sum, items);
                    var response = wb.UploadValues("https://api.turbo-smtp.com/api/mail/send", "POST", data);
                    return null;
                }
            }
            catch (Exception e) //post failure
            {
                return false;
            }
        }
    }
}