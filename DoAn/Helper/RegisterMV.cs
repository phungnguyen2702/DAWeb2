using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAn.Helper
{
    public class RegisterMV
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string StreetName { get; set; }
        public string StreetNumber { get; set; }
        public string City { get; set; }
        public int Country { get; set; }
        public DateTime BirdDay { get; set; }
    }
}