using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using Pivots.Commons.Network;
using Pivots.Commons;

namespace Pivots.Network
{
    /// <summary>
    ///PoliciesModule 的摘要说明
    /// </summary>
    public class PoliciesModule : IHttpModule
    {
        public void Dispose()
        {

        }

        public void Init(HttpApplication app)
        {
            app.AcquireRequestState += new EventHandler(Application_AcquireRequestState);
        }

        public void Application_AcquireRequestState(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;

            if (!app.Context.Request.Url.OriginalString.Contains("AppliationInitation"))
            {
                if (!CheckLicense(app.Context))
                {
                    app.CompleteRequest();
                    return;
                }

                if (!CheckSessionStrategy(app.Context))
                {
                    app.CompleteRequest();
                    return;
                }
            }
        }

        public bool CheckSessionStrategy(HttpContext context)
        {
            bool isCheckLogin = true;
            RouteData routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(context));
            if (routeData != null&&routeData.Values.ContainsKey("router"))
            {
                Router router = (Router)routeData.Values["router"];
                isCheckLogin = router.LoginVerity;
            }

            if (context.Session != null)
            {
                string url = context.Request.Url.AbsolutePath.ToLower();
                Object user = context.Session[Constants.SESSION_USER];
                if (isCheckLogin && user == null && (url.IndexOf(".aspx") > 0 || url.IndexOf(".ashx") > 0))
                {
                    context.Response.ContentType = "text/json";
                    context.Response.Charset = "utf-8";

                    const String code = "10002";
                    String msg = Pivots.SystemMsgInfo.Instance.GetMessage(code);
                    context.Response.Write(new BusinessProcessContext() { ClientNotificationType = ClientNotificationType.Notify, Message = msg, Code = code, ClientHandler = !Pivots.SystemMsgInfo.Instance.SystemMsgs.ContainsKey(code) ? String.Empty : Pivots.SystemMsgInfo.Instance[code].ClientHandler });
                    context.Response.End();
                    return false;
                }

                if (user != null)
                    Pivots.ContextHelper.ContextData.Add(Pivots.Commons.Constants.SESSION_USER, user);
            }

            return true;
        }

        public bool CheckLicense(HttpContext context)
        {

            if (!ConfigurationManager.IsLicenseVaild)
            {
                context.Response.ContentType = "text/json";
                context.Response.Charset = "utf-8";

                String msg = "许可证无效";
                context.Response.Write(new BusinessProcessContext() { ClientNotificationType = ClientNotificationType.Notify, Code = SystemInfoHelper.Value(), Message = msg, ClientHandler = "" });
                context.Response.End();
                return false;
            }

            if (ConfigurationManager.LicenseInfo.TrialDays != 0 && ConfigurationManager.LicenseInfo.TrialDateBegin.HasValue)
            {
                TimeSpan sp = DateTime.Now.Subtract(ConfigurationManager.LicenseInfo.TrialDateBegin.Value);
                if (sp.Days > ConfigurationManager.LicenseInfo.TrialDays)
                {
                    context.Response.ContentType = "text/json";
                    context.Response.Charset = "utf-8";

                    String msg = "许可证过期";
                    context.Response.Write(new BusinessProcessContext() { ClientNotificationType = ClientNotificationType.Notify, Message = msg, Code = SystemInfoHelper.Value(), ClientHandler = "" });
                    context.Response.End();
                    return false;
                }

            }

            return true;
        }
    }
}