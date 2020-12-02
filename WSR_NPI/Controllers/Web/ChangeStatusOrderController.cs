using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WSR_NPI.DataBase;

namespace WSR_NPI.Controllers.Web
{
    public class ChangeStatusOrderController : ApiController
    {
        
        public string Post(string token)
        {
            Context db = new Context();
            var user = db.Users.FirstOrDefault(x => x.Token.Equals(token));

            if (user != null)
            {
                var courier = db.Сouriers.FirstOrDefault(x => x.User.Id == user.Id);

                var order = db.Orders.FirstOrDefault(x => x.Id == courier.OrderId);
                order.Status = "Получен курьером";
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
