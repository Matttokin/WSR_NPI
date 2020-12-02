using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using WSR_NPI.DataBase;

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

                Context.Orders.Single(x => x.Id == model.OrderId).Status = "Ожидает курьера";
                Context.SaveChanges();

                return RedirectToAction("Index");
            }

            return PartialView("AssignCourierOrder", model);
        }

        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}