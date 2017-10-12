using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAn.Controllers
{
    public class MenuController : Controller
    {
        // GET: Menu
        public ActionResult GetMenu()
        {
            List<Models.Category> listCategory = null;
            using (var db = new Models.QLBHEntities())
            {
                listCategory = db.Categories.ToList();
                ViewBag.Ps = db.Species.ToList();
            }
            return PartialView(listCategory);
        }
    }
}