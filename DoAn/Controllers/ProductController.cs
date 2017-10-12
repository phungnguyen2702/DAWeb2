using DoAn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAn.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        public ActionResult Detail(int? id)
        {
            if (!id.HasValue)
                return RedirectToAction("Index", "Home");
            Product product = null;
            using (var db = new Models.QLBHEntities())
            {
                product = db.Products.Where(item => item.ProID == id)
                    .FirstOrDefault();
                if(product != null)
                {
                    ViewBag.PsameCat = db.Products.Where(item => item.CatID == product.CatID)
                   .Take(5).ToList();
                    ViewBag.PsamSpe = db.Products.Where(item => item.SpeID == product.SpeID)
                        .Take(5).ToList();
                    product.Viewer++;
                }
                db.SaveChanges();
            }
            if(product == null)
                return RedirectToAction("Index", "Home");
            return View(product);
        }

        // GET: ByCat
        public ActionResult ByCat(int? id, int page = 1)
        {
            List<Models.Product> listProduct;
            if (id.HasValue)
            {
                using (var db = new Models.QLBHEntities())
                {
                    ViewBag.CatN = db.Categories.Where(item => item.CatID == id).FirstOrDefault();
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

        // GET: ByCat
        public ActionResult BySpe(int? id, int page = 1)
        {
            List<Models.Product> listProduct;
            if (id.HasValue)
            {
                using (var db = new Models.QLBHEntities())
                {
                    ViewBag.Spe = db.Species.Where(it => it.ID == id).FirstOrDefault();
                    int n = db.Products.Where(it => it.SpeID == id).Count();
                    int recordsOfPage = 6;
                    int nPage = n / recordsOfPage;
                    nPage = n % recordsOfPage == 0 ? nPage : nPage + 1;
                    nPage = nPage == 0 ? 1 : nPage;

                    ViewBag.Pages = nPage;
                    ViewBag.CurentPage = page;
                    ViewBag.TotalPage = nPage;

                    listProduct = db.Products.Where(it => it.SpeID == id)
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
            return View("ByCat", listProduct);
        }
        static List<Models.Product> listProduct = new List<Models.Product>();
        static string ssearch;
        [HttpPost]
        public ActionResult search(string strSearch, int page = 1)
        {
            List<Models.Product> listP;
            listProduct.Clear();
            if (!string.IsNullOrEmpty(strSearch))
            {
                ssearch = strSearch;
                ViewBag.Stsearch = ssearch;
                using (var db = new Models.QLBHEntities())
                {
                    decimal pr;
                    decimal.TryParse(strSearch, out pr);
                    var pcat = db.Products.Where(it => it.Category.CatName.Contains(strSearch)).ToList();
                    listProduct.AddRange(pcat);
                    if (pr != 0)
                    {
                        var pPrice = db.Products.Where(item => item.Price > (pr - 2000000) && item.Price < (pr + 2000000))
                       .ToList().Except(pcat).ToList();
                        listProduct.AddRange(pPrice);
                    }
                    var pName = db.Products.Where(item => item.ProName.Contains(strSearch))
                        .ToList().Except(listProduct).ToList();
                    listProduct.AddRange(pName);
                    int n = listProduct.Count;
                    int recordsOfPage = 6;
                    int nPage = n / recordsOfPage;
                    nPage = nPage % recordsOfPage == 0 ? nPage : nPage + 1;
                    nPage = nPage == 0 ? 1 : nPage;

                    ViewBag.Pages = nPage;
                    ViewBag.CurentPage = page;
                    ViewBag.TotalPage = nPage;

                    listP = listProduct
                        .OrderBy(i => i.ProID)
                        .Skip((page - 1) * recordsOfPage)
                        .Take(recordsOfPage)
                        .ToList();
                }
                return View("ByCat", listP);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult search(int? page)
        {
            if (page.HasValue)
            {
                ViewBag.Stsearch = ssearch;
                List<Product> listP;

                int n = listProduct.Count;
                int recordsOfPage = 6;
                int nPage = n / recordsOfPage;
                nPage = n % recordsOfPage == 0 ? nPage : nPage + 1;
                nPage = nPage == 0 ? 1 : nPage;

                ViewBag.Pages = nPage;
                ViewBag.CurentPage = page;
                ViewBag.TotalPage = nPage;

                listP = listProduct
                    .OrderBy(i => i.ProID)
                    .Skip((page.Value - 1) * recordsOfPage)
                    .Take(recordsOfPage)
                    .ToList();

                return View("ByCat", listP);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}