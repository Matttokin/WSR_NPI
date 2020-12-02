using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using WSR_NPI.DataBase;
using WSR_NPI.DataBase.Models;

namespace WSR_NPI.Controllers.Web
{
    public class GetOrderByIdController : ApiController
    {

        public IEnumerable<Nomenclature> Get(int idOrder)
        {
            Context db = new Context();

            //получаем список товаров в заказе
            var orderNoms = db.OrderNoms.Where(x => x.OrderId == idOrder).Include(x => x.Nomenclature).Select(x => x.Nomenclature).ToList();

            //var order = db.Orders.Include(x => x.OrderNoms).FirstOrDefault(x => x.Id == idOrder);
            return orderNoms;
        }
    }
}