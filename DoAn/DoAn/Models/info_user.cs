//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DoAn.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class info_user
    {
        public int Id { get; set; }
        public string Company { get; set; }
        public Nullable<int> UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address_us { get; set; }
        public Nullable<int> Country { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string AboutMe { get; set; }
    
        public virtual Country Country1 { get; set; }
        public virtual User User { get; set; }
    }
}
