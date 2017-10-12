using DoAn.Filters;
using DoAn.Helper;
using DoAn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAn.Controllers
{
    [CheckLogin(RequiredPermission = 1)]
    public class ManagerController : Controller
    {
        // POST: RemoveOder
        [HttpPost]
        public ActionResult RemoveOder(int idO)
        {
            using (var db = new QLBHEntities())
            {
                var od = db.Orders.Where(item => item.OrderID == idO).FirstOrDefault();
                if (od != null)
                    od.IsDelete = true;
                db.SaveChanges();
            }

            return RedirectToAction("IndexAdmin", "Admin");
        }

        // POST: RemoveOder
        [HttpPost]
        public ActionResult Accept(int idA)
        {
            using (var db = new QLBHEntities())
            {
                var od = db.Orders.Where(item => item.OrderID == idA).FirstOrDefault();
                if (od != null)
                    od.IsVisited = true;
                db.SaveChanges();
            }

            return RedirectToAction("IndexAdmin", "Admin");
        }

        // Get: GetOrdered
        public ActionResult GetOrdered(int? id)
        {
            List<OrderDetail> listODD;
            if (id.HasValue)
            {
                using (var db = new QLBHEntities())
                {
                    listODD = db.OrderDetails.Include("Product")
                        .Where(item => item.OrderID == id).ToList();
                    var od = db.Orders.Where(item => item.OrderID == id).SingleOrDefault();
                    if (od != null)
                    {
                        ViewBag.Name = od.User.f_Name;
                        ViewBag.IDD = od.OrderID;
                        ViewBag.vs = od.IsVisited;
                    }
                }
                return View(listODD);
            }
            return RedirectToAction("IndexAdmin", "Admin");
        }


        // Get: RemoveOder
        public ActionResult getallordered(int page = 1)
        {
            List<OrderVM> listOderView;
            List<OrderVM> listOder;
            using (var db = new QLBHEntities())
            {
                //db.OrderDetails.SingleOrDefault().OrderID
                listOder = db.Orders
                    .Where(item => item.IsDelete == false && item.IsVisited == true)
                    .OrderByDescending(item => item.OrderDate)
                    .Select(item => new OrderVM()
                    {
                        OrderID = item.OrderID,
                        OrderedEmail = item.User.f_Email,
                        OrderedName = item.User.f_Name,
                        Total = item.Total
                    })
                    .ToList();
                int n = listOder.Count;
                int recordsOfPage = 10;
                int nPage = n / recordsOfPage;
                nPage = n % recordsOfPage == 0 ? nPage : nPage + 1;
                nPage = nPage == 0 ? 1 : nPage;

                ViewBag.Pages = nPage;
                ViewBag.CurentPage = page;
                ViewBag.TotalPage = nPage;

                foreach (var item in listOder)
                {
                    item.ProName = db.OrderDetails.Where(it => it.OrderID == item.OrderID)
                         .Select(it => it.Product.ProName).ToList();
                    item.QuanTity = db.OrderDetails.Where(it => it.OrderID == item.OrderID)
                         .Select(it => it.Quantity).ToList();
                }
                listOderView = listOder.Skip((page - 1) * recordsOfPage)
                        .Take(recordsOfPage)
                        .ToList();
            }
            return View(listOderView);
        }

        // Get: RemoveOder
        public ActionResult report()
        {           
            return View();
        }
        [HttpPost]
        public ActionResult getDataChart(int y = 2016)
        {
            List<ReportPro> p = new List<ReportPro>();
            int s = 0;             
            string tt ="";
            List<int> quantityofRes = new List<int>();
            List<int> QuanOder = new List<int>();
            using (var db = new QLBHEntities())
            {
                var c = db.Categories.OrderBy(it => it.CatID).ToList();              
                foreach (var item in c)
                {                   
                    ReportPro t = new ReportPro() { CatName = item.CatName };
                    var pro = db.OrderDetails.Include("Product")
                        .Where(it => it.Order.OrderDate.Year == y && it.Product.CatID == item.CatID)
                        .OrderBy(it => it.Order.OrderDate.Month)
                        .ToList();
                    var st = 0;
                    foreach (var it in new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 })
                    {
                        t.Month.Add(it);
                        var tm = pro.Where(itm => itm.Order.OrderDate.Month == it).Count();                       
                        st += tm;
                        t.QuanTity.Add(tm);                        
                    }
                    //s = st > s ? st : s;
                    if (st > s)
                    {
                        s = st;
                        tt = item.CatName;
                    }
                    p.Add(t);                    
                }
               
                foreach (var item in new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12})
                {
                    var qr = db.Users.Where(u => u.f_Day_Rerister.Value.Month == item).Count();
                    quantityofRes.Add(qr);
                    var qqo = db.Orders.Where(od => od.OrderDate.Day == item).Count();
                    QuanOder.Add(qqo);
                }
               
            }

            return Json(new { lbl = p.Select(i=>i.CatName).ToList(),sre=
                p.Select(i => i.QuanTity).ToList(),ma = s, pn = tt,
                us_q = new List<List<int>>{ quantityofRes, QuanOder }
            });
        }


        [HttpPost]
        public ActionResult getPercen(int y = 2016)
        {
            decimal total;
            List<decimal> kq = new List<decimal>();
            List<string> lbl = new List<string>();
            decimal q = 0;
            string name="";
            using (var db = new QLBHEntities())
            {
                var cat = db.Categories.ToList();
                total = db.Orders.Where(itm => itm.OrderDate.Year == y)
                    .Sum(itm => itm.OrderDetails.Sum(o => o.Amount));
                foreach (var item in cat)
                {
                    var tmp = db.OrderDetails.Where(id => id.Product.CatID == item.CatID
                    && id.Order.OrderDate.Year == 2016)
                        .ToList();
                    lbl.Add(string.Format("{0} %", Math.Round((tmp.Sum(i => i.Amount) / total) * 100,2)));
                    kq.Add(tmp.Sum(i=>i.Amount)/total);
                    if(q < tmp.Sum(i => i.Amount))
                    {
                        q = tmp.Sum(i => i.Amount);
                        name = item.CatName;
                    }
                }
            }
            return Json(new { kq=kq,lbl=lbl,qa = q,na = name});
        }
    }
}