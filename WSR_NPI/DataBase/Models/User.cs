using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSR_NPI.DataBase.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; }
        public int RoleId { get; set; }
        public string FIO { get; set; }
        public string Token { get; set; }
        public int Age { get; set; }
    }
}