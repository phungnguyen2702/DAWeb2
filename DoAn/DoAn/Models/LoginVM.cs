using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace DoAn.Models
{
    public class LoginVM
    {
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public bool Remember { get; set; }

    }
}