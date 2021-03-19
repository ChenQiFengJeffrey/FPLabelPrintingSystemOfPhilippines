using LabelPrintDAL;
using MobileDeviceWebAPI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace MobileDeviceWebAPI
{
    public class Global : System.Web.HttpApplication
    {

        public override void Init()
        {
            PostAuthenticateRequest += MvcApplication_PostAuthenticateRequest;
            base.Init();
        }

        void MvcApplication_PostAuthenticateRequest(object sender, EventArgs e)
        {
            //HttpContext.Current.SetSessionStateBehavior(
            //    SessionStateBehavior.Required);
        }

        protected void Application_Start()
        {
            //Pivots.ConfigurationManager.Instance.LoadConfig<Pivots.Data.Entity.InventoryRec>(false);
            AreaRegistration.RegisterAllAreas();
            //CacheWriter.GetInstance();//程序启动时，创建cache对象
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}