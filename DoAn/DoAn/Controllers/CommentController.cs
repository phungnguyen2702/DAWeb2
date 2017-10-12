using DoAn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAn.Controllers
{
    public class CommentController : Controller
    {
        // GET: GetComent
        public ActionResult GetComent(int id)
        {
            List<Comment> lstCmt = new List<Comment>();
            using (var Db = new QLBHEntities())
            {
                lstCmt = Db.Comments.Include("User").Where(item => item.ProId == id).ToList();
            }
            ViewBag.id = id;
            return PartialView(lstCmt);
        }
        [HttpPost]
        // GET: GetComent
        public JsonResult GetComent(int id, string cmt)
        {
            List<Comment> lstCmt = new List<Comment>();
            User u = Session["CurrentUser"] as Models.User;
            using (var Db = new QLBHEntities())
            {
                if (u != null && !string.IsNullOrEmpty(cmt))
                {
                    Db.Comments.Add(new Comment()
                    {
                        ProId = id,
                        UserId = u.f_ID,
                        Comment1 = cmt
                    });
                    Db.SaveChanges();
                }               
                lstCmt = Db.Comments.Include("User").Where(item => item.ProId == id).ToList();
            }
            return Json(u.f_Name);
        }
    }
}