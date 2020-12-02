using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using WSR_NPI.DataBase;
using WSR_NPI.Models;
using Newtonsoft.Json;
using WSR_NPI.DataBase.Models;

namespace WSR_NPI.Controllers
{
    [Authorize(Roles = "Работник склада")]
    public class WarehouseWorkerController : Controller
    {
        private Context Context = new Context();

        /// <summary>
        /// Список заказов
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            Context = new Context();
            var model = Context.Orders.Include(o => o.User).Where(x => x.Status.Equals("Принят") || x.Status.Equals("Комплектация начата") || x.Status.Equals("Отменен"));

            return View(model);
        }

        /// <summary>
        /// Содержимое заказа
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int id)
        {
            Context = new Context();
            var order = Context.Orders.Include(o => o.OrderNoms).Include(o => o.OrderNoms.Select(x => x.Nomenclature)).FirstOrDefault(o => o.Id == id);

            if (order != null)
            {
                return PartialView("Details", order);
            }

            return View("Index");
        }

        /// <summary>
        /// Удаление заказа
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            try
            {
                Context = new Context();
                var order = Context.Orders
                               .Include(x => x.OrderNoms)
                               .Include(x => x.OrderNoms.Select(y => y.Nomenclature))
                               .Single(x => x.Id == id);

                Context.Orders.Remove(order);
                Context.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// Смена статуса
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ChangeStatus(int id)
        {
            try
            {
                Context = new Context();
                var order = Context.Orders.Single(x => x.Id == id);

                if (order.Status.Equals("Принят"))
                {
                    order.Status = "Комплектация начата";
                }
                else
                {
                    order.Status = "Комплектация завершена";
                }

                var user = Context.Users.Single(x => x.Login.Equals(User.Identity.Name));

                if (SmartStatus(order))
                {
                    BlockChainManager.GenerateNextBlock(JsonConvert.SerializeObject(order), user.Id);
                }
                Context.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
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

        /// <summary>
        /// Смарт контракт для смены статуса
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        private bool SmartStatus(Order order)
        {
            Context = new Context();
            var o = Context.Orders.FirstOrDefault(x => x.Id == order.Id);

            if (o != null &&  ((o.Status.Equals("Принят") && order.Status.Equals("Комплектация начата")) || (o.Status.Equals("Комплектация начата") && order.Status.Equals("Комплектация завершена"))))
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