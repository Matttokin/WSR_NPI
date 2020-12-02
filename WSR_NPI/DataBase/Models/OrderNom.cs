using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSR_NPI.DataBase.Models
{
    public class OrderNom
    {
        public int Id { get; set; }
        public Order Order { get; set; }
        public Nomenclature Nomenclature { get; set; }
        public int OrderId { get; set; }
        public int NomenclatureId { get; set; }
        public int CountInOrder { get; set; }
    }
}