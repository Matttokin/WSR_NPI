using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using WSR_NPI.DataBase;

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
                var order = Context.Orders.Single(x => x.Id == id);

                if (order.Status.Equals("Принят"))
                {
                    order.Status = "Комплектация начата";
                }
                else
                {
                    order.Status = "Комплектация завершена";
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
    }
}