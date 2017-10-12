using DoAn.Helper;
using DoAn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAn.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult GetCart()
        {
            if (!UserContext.isLogin())
                return RedirectToAction("Index", "Home");
            var cart = UserContext.GetCart().Items;
            return View(cart);
        }
        // POST: Cart/Add
        [HttpPost]
        public ActionResult Add(int proId, int quantity)
        {
            using (var db = new Models.QLBHEntities())
            {
                var pro = db.Products.Where(p => p.ProID == proId && p.Quantity >= quantity)
                    .FirstOrDefault();
                if(pro != null)
                {
                    var item = new CartItem() { Quantity = quantity, product = pro };
                    UserContext.GetCart().AddItem(item);
                }
            }
            return RedirectToAction("Detail", "Product", new { id = proId });
        }

        // POST: Cart/AddIndex
        [HttpPost]
        public ActionResult AddIndex(int proId, int quantity,int curpage)
        {
            Product pro;
            using (var db = new Models.QLBHEntities())
            {
                pro = db.Products.Where(p => p.ProID == proId && p.Quantity >= quantity)
                    .FirstOrDefault();
                if(pro != null)
                {
                    var item = new CartItem() { Quantity = quantity, product = pro };
                    UserContext.GetCart().AddItem(item);
                }
            }
            return RedirectToAction("Index", "Home");
        }


        // POST: Cart/Remove
        [HttpPost]
        public ActionResult Remove(int proId)
        {
            UserContext.GetCart().RemoveItem(proId);
            return RedirectToAction("GetCart", "Cart");
        }

        // POST: Cart/Upadate
        [HttpPost]
        public ActionResult Update(int proId, int quantity)
        {
            Product pro;
            using (var db = new QLBHEntities())
            {
                pro = db.Products.Where(i => i.ProID == proId && i.Quantity >= quantity)
                    .FirstOrDefault();
            }
            if (pro != null && quantity >= 0 && pro.Quantity >= quantity)
                UserContext.GetCart().UpdateItem(proId, quantity);

            return RedirectToAction("GetCart", "Cart");
        }

        // POST: Cart/CheckOut
        [HttpPost]
        public ActionResult CheckOut()
        {
            using (var db = new QLBHEntities())
            {
                Order order = new Order()
                {
                    OrderDate = DateTime.Now,
                    UserID = UserContext.GetCurrentUser().f_ID,
                    Total = 0,
                    IsVisited = false,
                    IsDelete = false
                };
                Cart cart = UserContext.GetCart();
                decimal sum = 0;
                foreach (var item in cart.Items)
                {
                    var p = db.Products.Where(it => it.ProID == item.product.ProID)
                        .SingleOrDefault();
                    p.Quantity -= item.Quantity;
                    var detail = new OrderDetail()
                    {
                        ProID = item.product.ProID,
                        Quantity = item.Quantity,
                        Price = item.product.Price,
                        Amount = item.product.Price * item.product.Quantity
                    };
                    sum += detail.Amount;
                    order.OrderDetails.Add(detail);
                };
                order.Total = sum;
                db.Orders.Add(order);
                db.SaveChanges();
            }
            UserContext.GetCart().Items.Clear();
            return RedirectToAction("GetCart", "Cart");
        }

    }
}