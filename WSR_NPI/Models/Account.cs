using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WSR_NPI.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Заполните поле")]
        [Display(Name = "Логин")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Заполните поле")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }
    }

    public class RegisterModel
    {
        [Required(ErrorMessage = "Заполните поле")]
        [Display(Name = "Логин")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Заполните поле")]
        [Display(Name = "Ф.И.О")]
        public string Fio { get; set; }

        [Required(ErrorMessage = "Заполните поле")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Заполните поле")]
        [DataType(DataType.Password)]
        [Display(Name = "Повторите пароль")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Заполните поле")]
        [Display(Name = "Роль")]
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Заполните поле")]
        [Range(1, 110, ErrorMessage = "Некорректный возраст")]
        [Display(Name = "Возраст")]
        public int Age { get; set; }
    }
}