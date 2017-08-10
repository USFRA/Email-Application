using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Security;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [UserRoleAuthorize(Roles = "User")]
    public class HomeController : Controller
    {
        private EmailDbContext db = new EmailDbContext();

        public ActionResult Index()
        {
            //Session["AppURL"] = Request.Url.ToString();
            return View();
        }

        public String GetRoles()
        {
            string userRoles = db.Users.Where(x => x.Name == User.Identity.Name).FirstOrDefault().Roles;

            return userRoles;
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        //public String GetRoles()
        //{
        //    string userRoles = db.Users.Where(x => x.Name == User.Identity.Name).FirstOrDefault().Roles;

        //    return userRoles;
        //}

    }
}