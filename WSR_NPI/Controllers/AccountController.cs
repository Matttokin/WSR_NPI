using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using WSR_NPI.DataBase;
using WSR_NPI.DataBase.Models;
using WSR_NPI.Models;
using System.Web.Security;

namespace WSR_NPI.Controllers
{
    public class AccountController : Controller
    {
        private Context Context = new Context();

        // GET: Account
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Авторизация
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                // поиск пользователя в бд
                User user = null;
                user = Context.Users.Include(x => x.Role).FirstOrDefault(u => u.Login == model.Name && u.Password == model.Password);

                if (user != null && !user.Role.Name.Equals("Курьер"))
                {
                    FormsAuthentication.SetAuthCookie(model.Name, true);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Пользователя с таким логином и паролем нет");
                }
            }

            return View(model);
        }

        public ActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Регистрация
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = null;
                user = Context.Users.FirstOrDefault(u => u.Login == model.Login);


                if (user == null)
                {
                    // создаем нового пользователя
                    user = Context.Users.Add(new User { Login = model.Login, Password = model.Password, Age = model.Age, RoleId = model.RoleId, FIO = model.Fio });

                    var role = Context.Roles.Single(x => x.Id == model.RoleId);

                    if (role.Name.Equals("Курьер"))
                    {
                        Context.Сouriers.Add(new Сourier
                        {
                            Status = "Свободен",
                            UserId = user.Id
                        });
                    }

                    Context.SaveChanges();

                    user = Context.Users.Where(u => u.Login == model.Login && u.Password == model.Password).FirstOrDefault();

                    // если пользователь удачно добавлен в бд
                    if (user != null)
                    {
                        FormsAuthentication.SetAuthCookie(model.Login, true);
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Пользователь с таким логином уже существует");
                }
            }

            return View(model);
        }

        /// <summary>
        /// Выход
        /// </summary>
        /// <returns></returns>
        public ActionResult Logoff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}