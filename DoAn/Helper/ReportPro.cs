using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAn.Helper
{
    public class ReportPro
    {

        public string CatName { get; set; }
        public List<int> QuanTity { get; set; }
        public List<int> Month { get; set; }
      
        public ReportPro()
        {
            QuanTity = new List<int>();
            Month = new List<int>();
        }
    }
}