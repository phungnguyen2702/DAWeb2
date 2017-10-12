using DoAn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAn.Helper
{
    public class UserContext
    {
        static public bool isLogin()
        {
            var flag = HttpContext.Current.Session["isLogin"];
            if (flag == null || (bool)flag != true)
            {

                if (HttpContext.Current.Request.Cookies["user_id"] != null)
                {
                    int user_id = int.Parse(HttpContext.Current.Request.Cookies["user_id"].Value);
                    HttpContext.Current.Session["isLogin"] = true;

                    using (var db = new Models.QLBHEntities())
                    {
                        var CheckUser = db.Users.Where(item => item.f_ID == user_id)
                             .FirstOrDefault();
                        HttpContext.Current.Session["CurrentUser"] = CheckUser;
                    }
                    return true;
                }

                return false;
            }
            return true;
        }
        public static User GetCurrentUser()
        {
            return (User)HttpContext.Current.Session["CurrentUser"];
        }

        public static Cart GetCart()
        {
            Cart Result = (Cart)HttpContext.Current.Session["CurentCart"];
            if (Result == null)
            {
                Result = new Cart();
                HttpContext.Current.Session["CurentCart"] = Result;                 
            }
            return Result;
        }
        public static void Destroy()
        {
            HttpContext.Current.Session["isLogin"] = null;
            HttpContext.Current.Session["CurrentUser"] = null;
            HttpContext.Current.Response.Cookies["user_id"].Value = null;
            HttpContext.Current.Session["CurentCart"] = null;
            HttpContext.Current.Response.Cookies["user_id"].Expires = DateTime.Now.AddDays(-1);
        }
    }
}