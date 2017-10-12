using DoAn.Filters;
using DoAn.Helper;
using DoAn.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAn.Controllers
{
    [CheckLogin(RequiredPermission = 1)]
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult IndexAdmin(int page = 1)
        {
            List<OrderVM> listOderView;
            List<OrderVM> listOder;
            using(var db = new QLBHEntities())
            {
                //db.OrderDetails.SingleOrDefault().OrderID
                listOder = db.Orders
                    .Where(item => item.IsVisited == false && item.IsDelete == false)
                    .OrderByDescending(item=>item.OrderDate)
                    .Select(item=>new OrderVM() {OrderID = item.OrderID,OrderedEmail =item.User.f_Email,
                    OrderedName = item.User.f_Name,Total = item.Total})
                    .ToList();
                int n = listOder.Count;
                int recordsOfPage = 6;
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

        // GET: MenuAdmin
        public ActionResult GetMenu()
        {
            List<Models.Category> listCategory = null;
            using (var db = new Models.QLBHEntities())
            {
                listCategory = db.Categories.ToList();
            }
            return PartialView(listCategory);
        }

        // GET: Header Admin
        public ActionResult GetHeaderAdmin()
        {
            return PartialView();
        }


        // GET: GetProduct
        public ActionResult GetProduct(int? id, int page = 1)
        {
            List<Models.Product> listProduct;
            if (id.HasValue)
            {
                using (var db = new Models.QLBHEntities())
                {
                    var name = db.Categories.Where(it => it.CatID == id).SingleOrDefault();

                    if (name != null)
                        ViewBag.NameCat = name.CatName;

                    int n = db.Products.Where(it => it.CatID == id).Count();
                    int recordsOfPage = 6;
                    int nPage = n / recordsOfPage;
                    nPage = n % recordsOfPage == 0 ? nPage : nPage + 1;
                    nPage = nPage == 0 ? 1 : nPage;

                    ViewBag.Pages = nPage;
                    ViewBag.CurentPage = page;
                    ViewBag.TotalPage = nPage;

                    listProduct = db.Products.Where(it => it.CatID == id)
                        .OrderBy(i => i.ProID)
                        .Skip((page - 1) * recordsOfPage)
                        .Take(recordsOfPage)
                        .ToList();
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            return View(listProduct);
        }

        // GET: EditCategory
        public ActionResult EditCategory()
        {
            List<Category> listCat = null;
            using (var DB = new QLBHEntities())
            {
                listCat = DB.Categories.ToList();
            }
            return View(listCat);
        }

        // GET: EditCategory
        [HttpPost]
        public ActionResult EditCategory(Category cat)
        {
            List<Category> listCat = null;
            using (var DB = new QLBHEntities())
            {
                Category catEdit = DB.Categories.Where(item => item.CatID == cat.CatID)
                    .FirstOrDefault();
                if (catEdit != null)
                {
                    catEdit.CatName = cat.CatName;
                    DB.SaveChanges();
                    DB.Entry(catEdit).State = EntityState.Modified;
                }
                listCat = DB.Categories.ToList();
            }
            return View(listCat);
        }

        // GET: EditCategory
        [HttpPost]
        public ActionResult DeleteCat(int CatID)
        {
            List<Category> listCat = null;
            using (var DB = new QLBHEntities())
            {
                Category catDelete = DB.Categories
                    .Where(item => item.CatID == CatID)
                    .FirstOrDefault();
                if (catDelete != null)
                {
                    DB.Categories.Remove(catDelete);
                    DB.SaveChanges();
                }
                listCat = DB.Categories.ToList();
            }
            return View("EditCategory", listCat);
        }

        // Get: AddCategory
        public ActionResult AddCategory()
        {
            return View();
        }

        // POST: AddCategory
        [HttpPost]
        public ActionResult AddCategory(string catname)
        {
            Category newCat = new Category() { CatName = catname };
            using (var DB = new QLBHEntities())
            {
                DB.Categories.Add(newCat);
                DB.SaveChanges();
            }
            return RedirectToAction("IndexAdmin");
        }

        // Post: UpdateProduct/id
        public ActionResult UpdateProduct(int? id)
        {
            if (id.HasValue)
            {
                Product productUpdate = null;
                using (var DB = new QLBHEntities())
                {
                    productUpdate = DB.Products.Where(item => item.ProID == id)
                        .FirstOrDefault();

                    ViewBag.Cat = DB.Categories.ToList();                    
                }
                if (productUpdate != null)
                    return View(productUpdate);
                else
                    return RedirectToAction("IndexAdmin");
            }
            return RedirectToAction("IndexAdmin");
        }

        // POST: UpdateProduct
        [HttpPost]
        public ActionResult UpdateProduct(Product pro, HttpPostedFileBase fPmain,
            HttpPostedFileBase fPmainthums)
        {
            using (var DB = new QLBHEntities())
            {
                Product proEdit = DB.Products.Where(item => item.ProID == pro.ProID)
                    .FirstOrDefault();
                if (proEdit != null)
                {
                    string spDirPath = Server.MapPath("~/Images/Imgs/sp");
                    ServiceUpload.UploadImage(spDirPath,pro.ProID, fPmain, fPmainthums);
                    proEdit.ProName = pro.ProName;
                    proEdit.Quantity = pro.Quantity;
                    proEdit.Price = pro.Price;
                    proEdit.TinyDes = pro.TinyDes;
                    proEdit.FullDes += pro.FullDes;

                    DB.SaveChanges();
                    DB.Entry(proEdit).State = EntityState.Modified;
                }
                ViewBag.Cat = DB.Categories.ToList();
                return RedirectToAction("GetProduct",new {id = proEdit.CatID});
            }                                    
        }

        // POST: AddCategory
        [HttpPost]
        public ActionResult Remove(int proid,int curpage)
        {
            int catid = 1;       
            using (var DB = new QLBHEntities())
            {
                Product delPro = DB.Products.Where(item => item.ProID == proid)
                    .FirstOrDefault();
                if(delPro != null)
                {
                    //DB.Products.Remove(delPro);
                    //DB.SaveChanges();
                    catid = delPro.CatID;
                }
                else
                {
                    catid = DB.Categories.ToList().First().CatID;
                }
                
            }
            return RedirectToAction("GetProduct", "Admin", new { id = catid, page = curpage });
        }


        // Get:CreateProduct
        public ActionResult CreateProduct()
        {
            using(var db = new QLBHEntities())
            {
                ViewBag.Cat = db.Categories.ToList();
            }
            return View();
        }
        [HttpPost]
        public ActionResult CreateProduct(Product pro, HttpPostedFileBase fPmain,
            HttpPostedFileBase fPmainthums)
        {
            pro.TinyDes = pro.TinyDes == null ? string.Empty : pro.TinyDes;
            pro.FullDes = pro.FullDes == null ? string.Empty : pro.FullDes;
            string spDirPath = Server.MapPath("~/Images/Imgs/sp");
            using(var db = new QLBHEntities())
            {
                db.Products.Add(pro);
                db.SaveChanges();
            }
            ServiceUpload.UploadImage(spDirPath, pro.ProID, fPmain, fPmainthums);
            return RedirectToAction("GetProduct", new { id = pro.CatID });
        }
        [HttpPost]
        public ActionResult DeleteProduct(int proid,int pa)
        {
            using (var db = new QLBHEntities())
            {
                var p = db.Products.Where(item => item.ProID == proid).FirstOrDefault();
                if (p != null)
                {
                    string spDirPath = Server.MapPath("~/Images/Imgs/sp");
                    ServiceUpload.Remove(spDirPath,p.ProID);
                    db.Products.Remove(p);
                    db.SaveChanges();
                }
                return RedirectToAction("GetProduct", new { id = p.CatID, page = pa });
            }            
        }
    }
}