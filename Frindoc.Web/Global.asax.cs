using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Frindoc.Web.Models;
using WebMatrix.WebData;
using Logland.Client;

namespace Frindoc.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            // If you want Entity Framework to drop and regenerate your database
            // automatically whenever you change your model schema, add the following
            // code to the Application_Start method in your Global.asax file.
            // Note: this will destroy and re-create your database with every model change.
            System.Data.Entity.Database.SetInitializer(new SelfEvalInitializer());

            var ctx = new EvalDataContext();
            ctx.Database.Initialize(true);

            WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);

            Logger.ApplicationName = "FRINDOC";
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var lastErr = Server.GetLastError();
            Logger.Log(lastErr);
        }
    }
}