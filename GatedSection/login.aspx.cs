using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace FedAuth.CP
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //validate if user is already logged in
            string loggedInStr = HttpContext.Current.Request.ServerVariables["IsLoggedIn"];
            bool isLoggedIn = loggedInStr != null && loggedInStr.Equals("true", StringComparison.OrdinalIgnoreCase);

            //HttpContext.Current.Response.AddHeader("LoginPageIsLoggedIn", isLoggedIn.ToString());

            string path401 = ConfigurationManager.AppSettings["401path"] ?? "/401.aspx";
            
            if (isLoggedIn)
            {
                string nextPage = "/";
                if (Request.QueryString["nextPage"]!=null)
                {
                    nextPage = HttpContext.Current.Server.UrlDecode(Request.QueryString["nextPage"]);
                }
                HttpContext.Current.Response.AddHeader("LoginPageRedirectingTo", nextPage);
                Response.Redirect(nextPage);
            }
            else
            {
                Response.Redirect(path401);
            }
        }

    }
}