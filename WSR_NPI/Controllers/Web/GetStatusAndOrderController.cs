using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using WSR_NPI.DataBase;

namespace WSR_NPI.Controllers.Web
{
    public class GetStatusAndOrderController : ApiController
    {
        public ExportStatusAndOrder Post(string token)
        {
            Context db = new Context();
            var user = db.Users.FirstOrDefault(x => x.Token.Equals(token));

            if (user != null)
            {
                var courier = db.Сouriers.FirstOrDefault(x => x.User.Id == user.Id);

                var order = db.Orders.Include(x => x.OrderNoms).FirstOrDefault(x => x.Id == courier.OrderId);
                var orderNoms = db.OrderNoms.Where(x => x.OrderId == courier.OrderId).Include(x => x.Nomenclature).ToList();
                List<ExportNomenclature> len = new List<ExportNomenclature>();
                foreach (var orderNomsElem in orderNoms)
                {
                    ExportNomenclature en = new ExportNomenclature();
                    en.Name = orderNomsElem.Nomenclature.Name;
                    en.CountInOrder = orderNomsElem.CountInOrder;
                    len.Add(en);
                }
                ExportStatusAndOrder esao = new ExportStatusAndOrder { Status = courier.Status, Order = new OrderExport { Adres = order.Adres, Nom = len } };
                return esao;
            }
            else
            {
                return null;
                //
            }
        }
    }
    public class ExportStatusAndOrder
    {
        public string Status { get; set; }
        public OrderExport Order { get; set; }
    }
    public class OrderExport
    {
        public string Adres { get; set; }
        public List<ExportNomenclature> Nom { get; set; }
    }
    public class ExportNomenclature
    {
        public string Name { get; set; }
        public int CountInOrder { get; set; }
    }
}