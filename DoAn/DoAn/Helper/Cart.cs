using DoAn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAn.Helper
{
    public class Cart
    {
        public List<CartItem> Items { get; set; }
        public Cart()
        {
            Items = new List<CartItem>();
        }
        public int SumQuantity()
        {
            return Items.Sum(item => item.Quantity);
        }
        public void AddItem(CartItem item)
        {
            var check = Items.Where(i => i.product.ProID == item.product.ProID).FirstOrDefault();
            if(check != null)
            {
                check.Quantity += item.Quantity;
            }
            else
                Items.Add(item);
        }

        public void RemoveItem(int proId)
        {
            var check = Items.Where(i => i.product.ProID == proId).FirstOrDefault();
            if (check != null)
            {
                this.Items.Remove(check);
            }
        }


        public void UpdateItem(int proId, int quantity)
        {
            var check = Items.Where(i => i.product.ProID == proId).FirstOrDefault();
            if (check != null)
            {
                check.Quantity = quantity;
            }
        }
    }

    public class CartItem
    {
        public Product product { get; set; }
        public int Quantity { get; set; }
    }
}