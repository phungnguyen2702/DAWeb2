using DoAn.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using DoAn.Models;
using BotDetect.Web.Mvc;
using DoAn.Helper;
using System.Text.RegularExpressions;

namespace DoAn.Controllers
{
    public class HeaderController : Controller
    {
        // GET: Header
        public ActionResult GetNavbarHeader()
        {
            return PartialView();
        }
        // GET: Header/Login
        public ActionResult Login()
        {
            return View();
        }
        // POST: Header/Login
        [HttpPost]
        public ActionResult Login([Bind(Include = "UserName,PassWord,Remember")] Models.LoginVM user)
        {
            string txtPassword = user.PassWord;
            string RawPassword;
            Helper.UserContext.Destroy();

            using (var md5Hash = MD5.Create())
            {
                RawPassword = Helper.StringUtils.GetMd5Hash(md5Hash, txtPassword);
                ///RawPassword.ToLower();
            }
            using (var db = new Models.QLBHEntities())
            {
                var CheckUser = db.Users.Where(item => item.f_Username == user.UserName
                && item.f_Password == RawPassword)
                     .FirstOrDefault();
                if (CheckUser != null)
                {
                    Session["isLogin"] = true;
                    Session["CurrentUser"] = CheckUser;
                    if (user.Remember == true)
                    {
                        Response.Cookies["user_id"].Value = CheckUser.f_ID.ToString();
                        ///System.Diagnostics.Debug.Write(Response.Cookies["user_id"].Value+"\n\n\n");
                        Response.Cookies["user_id"].Expires = DateTime.Now.AddDays(7);
                    }
                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.MgsErorrAccount = "Sai Tài Khoản Hoặc Mật Khẩu";
            return View();
        }

        // GET: Header/Logout
        [HttpPost]
        public ActionResult Logout()
        {
            Helper.UserContext.Destroy();
            return RedirectToAction("Index", "Home");
        }

        // GET: Header/ViewProfile
        [CheckLogin]
        public ActionResult ViewProfile()
        {
            Models.User currentUser = Session["CurrentUser"] as Models.User;
            if (currentUser != null)
            {
                Models.info_user us = null;
                using(var db = new QLBHEntities())
                {
                    us = db.info_user.Include("User").Include("Country1").Where(item => item.UserId == currentUser.f_ID).SingleOrDefault();
                }
                return View(us);
            }
            else
               return RedirectToAction("Login");
        }

        [CheckLogin]
        [HttpPost]
        public ActionResult ViewProfile(info_user user,string zipcode)
        {
            Models.User currentUser = Session["CurrentUser"] as Models.User;
            if (currentUser != null)
            {
                Models.info_user us = null;
                using (var db = new QLBHEntities())
                {
                    us = db.info_user.Include("User").Include("Country1")
                        .Where(item => item.UserId == currentUser.f_ID)
                        .SingleOrDefault();
                    us.AboutMe = user.AboutMe;
                    us.Address_us = user.Address_us;
                    us.City = user.City;
                    us.FirstName = user.FirstName;
                    us.LastName = user.LastName;
                    us.Company = user.Company;
                    us.User.f_ZipCode = zipcode;
                    db.SaveChanges();
                    db.Entry(us).State = System.Data.Entity.EntityState.Modified;
                }
                return View(us);
            }
            else
                return RedirectToAction("Login");
        }

        // GET: Header/DoimatKhau
        [CheckLogin]
        public ActionResult EditPassWord()
        {
            return View();
        }

        [CheckLogin]
        [HttpPost]
        public ActionResult EditPassWord(string oldPW, string newPW, string confirmPW)
        {
            string RawPassword;
            using (var md5Hash = MD5.Create())
            {
                RawPassword = Helper.StringUtils.GetMd5Hash(md5Hash, oldPW);
            }
            Models.User currentUser = Session["CurrentUser"] as Models.User;
            if (currentUser != null)
            {
                if(currentUser.f_Password != RawPassword)
                {
                    ViewBag.error = "Sai Mật Khẩu Cũ";
                    return View();
                }
                else
                {
                    if (newPW == confirmPW)
                    {
                        using (var md5Hash = MD5.Create())
                        {
                            RawPassword = Helper.StringUtils.GetMd5Hash(md5Hash, newPW);
                        }
                        using(var Db = new QLBHEntities())
                        {
                            var u = Db.info_user.Include("User").Include("Country1")
                                .Where(item => item.UserId == currentUser.f_ID)
                                .SingleOrDefault();
                            if (u != null)
                            {
                                u.User.f_Password = RawPassword;
                                ViewBag.sspw = true;
                                Db.SaveChanges();
                                return View("ViewProfile", u);
                            }
                        }
                    }
                    ViewBag.error = "Mật Khẩu Không Khớp";
                    return View();
                }
             
            }
            else
                return RedirectToAction("Login");
        }


        // GET: Header/Register
        public ActionResult Register()
        {
            using (var db = new QLBHEntities())
            {
                ViewBag.Countries = db.Countries.ToList();
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [CaptchaValidation("CaptchaCode", "ExampleCaptcha", "Incorrect CAPTCHA code!")]
        public ActionResult Register(RegisterMV model, HttpPostedFileBase imgUser)
        {
            if (!ModelState.IsValid)
            {
                // TODO: Captcha validation failed, show error message
                using (var DB = new QLBHEntities())
                {
                    ViewBag.Countries = DB.Countries.ToList();
                }
            }
            else
            {
                // TODO: Captcha validation passed, proceed with protected action
                string txtpw = model.Password;
                string CWPW = model.ConfirmPassword;
                if (!string.IsNullOrEmpty(txtpw) && txtpw.CompareTo(CWPW) == 0 && !string.IsNullOrEmpty(CWPW))
                {
                    Models.User us = new Models.User();
                    using (var md5Hash = MD5.Create())
                    {
                        string RawPassword = Helper.StringUtils.GetMd5Hash(md5Hash, txtpw);
                        us.f_Password = RawPassword;
                    }
                    us.f_Username = model.UserName;
                    us.f_Name = model.FirstName;
                    us.f_DOB = model.BirdDay;
                    us.f_Email = model.Email;
                    us.f_Day_Rerister = DateTime.Now;
                    info_user ifu = new info_user();
                    ifu.FirstName = model.FirstName;
                    ifu.LastName = model.LastName;
                    ifu.Country = model.Country;
                    ifu.City = model.City;
                    ifu.Address_us = model.StreetNumber + model.StreetName;
                    ifu.User = us;
                    //us.f_DOB
                    using (var DB = new QLBHEntities())
                    {
                        DB.Users.Add(us);
                        DB.info_user.Add(ifu);
                        DB.SaveChanges();
                        ViewBag.Countries = DB.Countries.ToList();
                    }
                    Session["isLogin"] = true;
                    Session["CurrentUser"] = us;
                    RedirectToAction("ViewProfile");
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult Checkemail(string email)
        {
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            if (string.IsNullOrEmpty(email))
            {
                return Json(false);
            }
            else if (!regex.IsMatch(email))
            {
                return Json(false);
            }
            else
            {
                bool rsl = false;
                using (var db = new QLBHEntities())
                {
                    var user = db.Users.Where(item => item.f_Email == email).FirstOrDefault();
                    if (user == null)
                        rsl = true;
                }
                return Json(rsl);
            }
        }


        [HttpPost]
        public ActionResult Checkusername(string username)
        {

            if (string.IsNullOrEmpty(username))
            {
                return Json(false);
            }
            else if (username.Length < 6)
            {
                return Json(false);
            }
            else
            {
                bool rsl = false;
                using (var db = new QLBHEntities())
                {
                    var user = db.Users.Where(item => item.f_Username == username).FirstOrDefault();
                    if (user == null)
                        rsl = true;
                }
                return Json(rsl);
            }
        }
    }
}