using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace WebApplication1
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        //protected void Session_Start(Object sender, EventArgs e)
        //{
        //    HttpContext.Current.Session["AppURL"] = Request.Url.ToString();
        //    //HttpContext.Current.Session["beingredirected"] = "false";
        //    //Session["AppURL"] = Request.Url.ToString();
        //}
    }
}
