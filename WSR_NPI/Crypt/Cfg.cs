using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSR_NPI.Crypt
{
    public class Cfg
    {
        private static string _Key = "ts1XY2dmkD8hQqVqzaegf/xY0MXHGW60F12F4s+O+Vc=";
        private static string _IV = "QbRbjjzsjq3XibwSSdSyeg==";

        public static string GetKey()
        {
            return _Key;
        }

        public static string GetIV()
        {
            return _IV;
        }
    }
}