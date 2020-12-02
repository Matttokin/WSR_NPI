namespace WSR_NPI.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Text;
    using WSR_NPI.DataBase;
    using WSR_NPI.DataBase.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<WSR_NPI.DataBase.Context>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        //seed позволяет при запуске приложения проверить наличие необходимых данных и внести их при их отсутствии 
        protected override void Seed(WSR_NPI.DataBase.Context context)
        {

            createRole(context, "Менеджер по продажам");
            createRole(context, "Работник склада");
            createRole(context, "Менеджер по доставке");
            createRole(context, "Курьер");

            createNom(context, "Бумага А4 500л.", 31);
            createNom(context, "Кирпич", 88);
            createNom(context, "Самокат", 99);
            createNom(context, "Кактус", 7);
            createNom(context, "Гитара", 6);
            createNom(context, "Xiaomi Mi Mix 3", 5);
            createNom(context, "Ноутбук MSI", 2);
            createNom(context, "Тетрадь 96л.", 28);
            createNom(context, "Ручка синия", 345);
            createNom(context, "Сахар 10кг.", 44);

            createUser(context, "ManProd", "1234qW", "Иванов И.И.", "Менеджер по продажам", 20);
            createUser(context, "RabSkl", "1234qW", "Семенов С.С.", "Работник склада", 22);
            createUser(context, "ManDost", "1234qW", "Кузнецов К.К.", "Менеджер по доставке", 24);
            createUser(context, "Kur", "1234qW", "Дубровский Д.Д.", "Курьер", 18);

            InitBlock(context);

        }
        public void startSeed()
        {
            Seed(new Context());
        }
        public void createRole(WSR_NPI.DataBase.Context db, string nameRole)
        {
            var rol = db.Roles.FirstOrDefault(x => x.Name.Equals(nameRole));

            if (rol == null)
            {
                db.Roles.Add(new Role { Name = nameRole });
                db.SaveChanges();
            }
        }

        //далее находятся методы для записи строк в бд
        public void createUser(WSR_NPI.DataBase.Context db, string login,string password,string fio, string roleName, int age)
        {
            User user = null;
            user = db.Users.FirstOrDefault(u => u.Login == login);


            if (user == null)
            {
                var RoleId = db.Roles.FirstOrDefault(x => x.Name.Equals(roleName)).Id;
                // создаем нового пользователя
                Random rd = new Random();
                user = db.Users.Add(new User { Login = login, Password = password, Age = age, RoleId = RoleId, FIO = fio,Token = CreateMD5(login + password +rd.Next(0,99).ToString()) });

                
                if (roleName.Equals("Курьер"))
                {
                    db.Сouriers.Add(new Сourier
                    {
                        Status = "Свободен",
                        UserId = user.Id
                    });
                }

                db.SaveChanges();

                user = db.Users.Where(u => u.Login == login && u.Password == password).FirstOrDefault();

                
            }
        }

        public void createNom(WSR_NPI.DataBase.Context db, string nameNom, int count)
        {
            var rol = db.Nomenclatures.FirstOrDefault(x => x.Name.Equals(nameNom));

            if (rol == null)
            {
                db.Nomenclatures.Add(new Nomenclature { Name = nameNom, Count = count });
                db.SaveChanges();
            }
        }

        //получаем MD5 хеш от строки
        public static string CreateMD5(string input)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        public void InitBlock(WSR_NPI.DataBase.Context db)
        {
            var block = db.Blocks.FirstOrDefault(b => b.Index == 1);

            if (block == null)
            {
                db.Blocks.Add(new Block
                {
                    Index = 1,
                    TimeStamp = DateTime.UtcNow.Ticks,
                    Data = "genesis block!",
                    Hash = "816534932c2b7154836da6afc367695e6337db8a921823784c14378abed4f7d7",
                    PreviousHash = "0"
                });

                db.SaveChanges();
            }
        }
    }
}
