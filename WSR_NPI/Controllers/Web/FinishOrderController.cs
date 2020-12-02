using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using WSR_NPI.DataBase;

namespace WSR_NPI.Controllers.Web
{
    public class FinishOrderController : ApiController
    {
        public string Post(string token)
        {
            Context db = new Context();
            var user = db.Users.FirstOrDefault(x => x.Token.Equals(token));

            if (user != null)
            {
                var courier = db.Сouriers.FirstOrDefault(x => x.User.Id == user.Id);
                courier.Status = "Свободен";
                var order = db.Orders.FirstOrDefault(x => x.Id == courier.User.Id);
                order.Status = "Доставлен";
                courier.OrderId = null;
                db.SaveChanges();

                return "Успешно";
            }
            else
            {
                return null;
            }
        }
    }
}