using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using System.Text;
using System.Web.Routing;

namespace WebApplication1.Security
{
    public class UserRoleAuthorize : AuthorizeAttribute
    {
        private EmailDbContext db = new EmailDbContext();
        

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            //It seperates the comma separated roles.
            //The data comes from the controller
            var roles = Roles.Split(',');
            string userNmae = System.Web.HttpContext.Current.User.Identity.Name;
            //string userNmae = User.Identity.Name.Substring(6);
            var user = db.Users.Any(u => u.Name == userNmae);

            if (httpContext.User.Identity.IsAuthenticated)
            {
                //Here I check if the user is in the role, you can have your own logic. The data is gotten from DB.
                var userRoles = db.Users.Where(u => u.Name == userNmae);


                //HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(System.Security.Principal.IIdentity, userRoles.ToList());

                foreach (var role in roles)
                    if (userRoles.Any(obj => obj.Roles.Contains(role)))
                        return true;
            }
            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            //you can change to any controller or html page.
            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(
                    new
                    {
                        controller = "Error",
                        action = "Unauthorized"
                    })
                );

        }

        //public override void OnAuthorization(AuthorizationContext filterContext)
        //{

        //    string userNmae = System.Web.HttpContext.Current.User.Identity.Name;

        //    bool userValid = db.Users.Any(u => u.Name == userNmae);


        //    HttpContextBase.User.Identity.IsAuthenticated = false;
        //    //if(userValid)
        //    //{
        //    //    HttpContextBase.User.Identity.IsAuthenticated = true;
        //    //}
        //    //else
        //    //{
        //    //    HttpContextBase.User.Identity.IsAuthenticated = false;
        //    //}


        //    base.OnAuthorization(filterContext);
        //}

        
    }

}