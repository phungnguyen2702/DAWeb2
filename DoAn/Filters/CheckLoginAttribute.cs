using DoAn.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAn.Filters
{
    public class CheckLoginAttribute : ActionFilterAttribute
    {
        public int RequiredPermission { get; set; }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (! UserContext.isLogin())
            {
                filterContext.Result = new RedirectResult("~/Header/Login");
                return;
            }
            if(UserContext.GetCurrentUser().f_Permission < RequiredPermission)
            {
                filterContext.Result = new HttpUnauthorizedResult();
                return;
            }
            base.OnActionExecuting(filterContext);
        }
    }
}