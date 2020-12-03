using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSR_NPI.Models
{
    public class Picture
    {
        public int Id { get; set; }
        public string Name { get; set; } // название картинки
        public byte[] Image { get; set; }

        public bool IsView { get; set; }
    }
}