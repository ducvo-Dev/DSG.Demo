using DSG.WebAPI.App_Start;
using DSG.WebAPI.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
namespace DSG.WebAPI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            Mappings.AutoMapperConfiguration.Init();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            Context.Response.AppendHeader("Access-Control-Allow-Credentials", "true");
            var referrer = Request.UrlReferrer;
            if (Context.Request.Path.Contains("signalr/") && referrer != null)
            {
                Context.Response.AppendHeader("Access-Control-Allow-Origin", referrer.Scheme + "://" + referrer.Authority);
            }
        }
    }
}
