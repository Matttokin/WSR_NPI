using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WSR_NPI.DataBase.Models;

namespace WSR_NPI.DataBase
{
    public class Context : DbContext
    {
        //инициализируем соединение с бд
        public Context()
            : base("DBConnection")
        { }

        
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Nomenclature> Nomenclatures { get; set; }
        public DbSet<OrderNom> OrderNoms { get; set; }
        public DbSet<Сourier> Сouriers { get; set; }

        public DbSet<Block> Blocks { get; set; }
    }
}