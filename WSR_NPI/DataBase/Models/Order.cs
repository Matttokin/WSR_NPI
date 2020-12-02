using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSR_NPI.DataBase.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string Adres { get; set; }
        public string Status { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public ICollection<OrderNom> OrderNoms { get; set; }


    }
}