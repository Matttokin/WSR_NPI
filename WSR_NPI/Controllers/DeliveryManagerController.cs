using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using WSR_NPI.DataBase;
using Newtonsoft.Json;
using WSR_NPI.Models;
using WSR_NPI.DataBase.Models;

namespace WSR_NPI.Controllers
{
    [Authorize(Roles = "Менеджер по доставке")]
    public class DeliveryManagerController : Controller
    {
        private Context Context = new Context();

        // GET: DeliveryManager
        public ActionResult Index()
        {
            var model = Context.Orders.Include(o => o.User).Where(x => x.Status.Equals("Комплектация завершена"));

            return View(model);
        }

        /// <summary>
        /// Содержимое заказа
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int id)
        {
            var order = Context.Orders.Include(o => o.OrderNoms).Include(o => o.OrderNoms.Select(x => x.Nomenclature)).FirstOrDefault(o => o.Id == id);

            if (order != null)
            {
                return PartialView("Details", order);
            }

            return View("Index");
        }

        /// <summary>
        /// Список курьеров
        /// </summary>
        /// <returns></returns>
        public ActionResult Couriers()
        {
            var model = Context.Сouriers.Include(x => x.User);
            return View(model);
        }

        /// <summary>
        /// Назначение курьера на заказ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AssignCourierOrder(int id)
        {
            var model = new WSR_NPI.DataBase.Models.Сourier
            {
                OrderId = id
            };

            return PartialView("AssignCourierOrder", model);
        }

        /// <summary>
        /// Назначение курьера на заказ
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AssignCourierOrder(WSR_NPI.DataBase.Models.Сourier model)
        {
            if (ModelState.IsValid)
            {
                var c = Context.Сouriers.Single(x => x.Id == model.Id);
                c.Status = "Доставляет";
                c.OrderId = model.OrderId;

                var order = Context.Orders.Single(x => x.Id == model.OrderId);
                order.Status = "Ожидает курьера";
                var user = Context.Users.Single(x => x.Login.Equals(User.Identity.Name));

                if (SmartCourier(order, model)) 
                {
                    BlockChainManager.GenerateNextBlock(JsonConvert.SerializeObject(order), user.Id);
                }
                Context.SaveChanges();

                return RedirectToAction("Index");
            }

            return PartialView("AssignCourierOrder", model);
        }

        /// <summary>
        /// Смарт контракт для назначения курьера на заказ
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        private bool SmartCourier(Order order, WSR_NPI.DataBase.Models.Сourier model)
        {
            Context = new Context();
            var o = Context.Orders.FirstOrDefault(x => x.Id == order.Id);
            var c = Context.Сouriers.Single(x => x.Id == model.Id);

            if (o != null && o.Status.Equals("Комплектация завершена") && c.Status.Equals("Свободен"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}