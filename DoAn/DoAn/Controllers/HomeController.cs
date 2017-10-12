using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAn.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index(int? id)
        {
            List<Models.Product> listProduct;
            using (var db = new Models.QLBHEntities())
            {
                if (id.HasValue)
                {
                    listProduct = db.Products.Where(it => it.CatID == id).ToList();  
                }
                else
                {   
                    listProduct = db.Products.ToList(); 
                }
            }
            return View(listProduct);
        } 
        
    }
}