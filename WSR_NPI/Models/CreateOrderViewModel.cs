using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WSR_NPI.Models
{
    public class CreateOrderViewModel
    {
        [Display(Name = "Адрес")]
        public string Adress { get; set; }

        public List<CreateOrderNum> OrderNums { get; set; }
    }

    public class CreateOrderNum
    {
        public string Name { get; set; }

        public bool IsBuy { get; set; }

        /// <summary>
        /// Остаток на складе
        /// </summary>
        public int Count { get; set; }

        public int CountBuy { get; set; }
    }
}