using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAn.Helper
{
    public class OrderVM
    {
        public int OrderID { get; set; }
        public string OrderedName { get; set; }
        public string OrderedEmail { get; set; }
        public List<string> ProName { get; set; }
        public List<int> QuanTity { get; set; }
        public decimal Total { get; set; }
        public OrderVM()
        {
            ProName = new List<string>();
        }

    }
}