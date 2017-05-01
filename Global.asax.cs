using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Web.Routing;
using System.Web.Mvc;
using System.Web.Http;


namespace FedAuth.CP
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
                       
        }

        public class AddCustomHeaderFilter : System.Web.Http.Filters.ActionFilterAttribute
        {
           
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_AuthorizeRequest(object sender, EventArgs e)
        {
            //validate SSO claims and return 401 if access is denied
            if (!FedAuthHelper.isValidSSOClaim())
            {
                Response.StatusCode = 401;
                Response.End();
            }

            //validate pathrough FedAuth access and return 401 if access is denied
            if (!FedAuthHelper.isValidSectionAccess())
            {
                string path401 = System.Configuration.ConfigurationManager.AppSettings["401path"] ?? "/401.aspx";
                Response.Redirect(path401);
            }
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}