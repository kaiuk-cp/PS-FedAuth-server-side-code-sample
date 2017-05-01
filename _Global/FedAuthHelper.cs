using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Configuration;

namespace FedAuth.CP
{
    public class FedAuthHelper
    {
        /// <summary>
        /// Validate Stage SSO Claims
        /// </summary>
        public static bool isValidSSOClaim()
        {

            string SSOProfileClaimName = ConfigurationManager.AppSettings["SSOProfileClaimName"] ?? "PROFILE_TYPE";

            //TEST ONLY - allow SSO validation on aspx and PDF files only to avoid js/css/images 401 responses
            //DELETE BEFORE GOING LIVE
            //if (!HttpContext.Current.Request.Path.ToLower().EndsWith(".aspx") && !HttpContext.Current.Request.Path.ToLower().EndsWith(".pdf"))
            //{
            //return true;
            //}
            //TEST ONLY - set claim value from query string
            //DELETE BEFORE GOING LIVE
            //HttpContext.Current.Request.ServerVariables[SSOProfileClaimName] = HttpContext.Current.Request.QueryString["profile_type"] ?? HttpContext.Current.Request.ServerVariables["PROFILE_TYPE"];

            //Do not remove below this line

            string SSOValidationRequired = ConfigurationManager.AppSettings["SSOEnabled"] ?? "";
            //Allow all requests if SSO is not enabled in web.config
            if (SSOValidationRequired != "true")
                return true;

            //read claim from ServerVariables 
            //Value = E (FTE)
            //Value = V (Temporary Employees, Vendors, Contractors)
            string SSOCurrentEmplType = HttpContext.Current.Request.ServerVariables[SSOProfileClaimName] ?? "";

            if (isValidProfileType(SSOCurrentEmplType))
                return true;

            //Allow requests to users on roster, roster is managed in the CMS
            string SSOisUserOnRoaster = HttpContext.Current.Request.ServerVariables["isonroster"] ?? "";

            if (SSOisUserOnRoaster.ToLower() == "true")
                return true;

            return false;
        }





        /// <summary>
        /// Validate SectionAccess access
        /// </summary>
        public static bool isValidSectionAccess()
        {

            string SSOProfileClaimName = ConfigurationManager.AppSettings["SSOProfileClaimName"] ?? "PROFILE_TYPE";
            string pathLogin = ConfigurationManager.AppSettings["SSOSectionAccessLoginPage"] ?? "/login.aspx";
            if (HttpContext.Current.Request.FilePath==pathLogin)
            {
                return true;
            }

            //TEST ONLY - set claim value from query string
            //DELETE BEFORE GOING LIVE
            //HttpContext.Current.Request.ServerVariables[SSOProfileClaimName] = HttpContext.Current.Request.QueryString["profile_type"] ?? HttpContext.Current.Request.ServerVariables["PROFILE_TYPE"];


            //Do not remove below this line
            string SectionAccessSSOValidationRequired = ConfigurationManager.AppSettings["SSOSectionAccessEnabled"] ?? "";
            //Allow all requests if SSO is not enabled in web.config

            if (SectionAccessSSOValidationRequired != "true")
                return true;

            //HttpContext.Current.Response.AddHeader("SSOSectionAccessValidationRequired", SectionAccessSSOValidationRequired);

            string SSOSectionAccessFolder = ConfigurationManager.AppSettings["SSOSectionAccessFolder"] ?? "/GatedSection/producers";
            //HttpContext.Current.Response.AddHeader("SSOSectionAccessFolder", SSOSectionAccessFolder);
            //Allow requests outside of the SectionAccess folder
            if (SSOSectionAccessFolder != "/")
            {
                if (!HttpContext.Current.Request.Path.ToLower().Contains(SSOSectionAccessFolder))
                    return true;
            }

            //Allow requests to users on roster, roster is managed in the CMS
            string SSOisUserOnRoaster = HttpContext.Current.Request.ServerVariables["isonroster"] ?? "";
            //HttpContext.Current.Response.AddHeader("SSOSectionAccessIsUserOnRoaster", SSOisUserOnRoaster);
            if (SSOisUserOnRoaster.ToLower() == "true")
                return true;

            //validate if user is already logged in
            string loggedInStr = HttpContext.Current.Request.ServerVariables["IsLoggedIn"];

            //HttpContext.Current.Response.AddHeader("SSOSectionAccessServerVariablesIsLoggedIn", loggedInStr ?? "");
            bool isLoggedIn = loggedInStr != null && loggedInStr.Equals("true", StringComparison.OrdinalIgnoreCase);

            //HttpContext.Current.Response.AddHeader("SSOSectionAccessIsLoggedIn", isLoggedIn.ToString());

            string SSOCurrentEmplType = HttpContext.Current.Request.ServerVariables[SSOProfileClaimName] ?? "";
            //HttpContext.Current.Response.AddHeader("SSOSectionAccessCurrentProfileType", SSOCurrentEmplType);
            if (isLoggedIn || SSOCurrentEmplType != "")
            {
                //read claim from ServerVariables 
                //Value = E (FTE), V (Temporary Employees, Vendors, Contractors), A (Agent)
                
                if (isValidProfileType(SSOCurrentEmplType)) {
                    //HttpContext.Current.Response.AddHeader("SSOProfileTypeIsValid", "true");
                    return true; 
                }
                else
                {
                    //HttpContext.Current.Response.AddHeader("SSOProfileTypeIsValid", "false");
                }
            }
            else
            {
                string showProvidersDropdown = HttpContext.Current.Request.QueryString["showProviderDropdown"] ?? "";
                string SSOSectionAccessDefaultProvider = ConfigurationManager.AppSettings["SSOSectionAccessDefaultProvider"] ?? "Crownpeak";
                string defaultProvider = showProvidersDropdown == "true" ? "" : "&providerName=" + SSOSectionAccessDefaultProvider;

                //Need to double encode the nextPage URL to avoid issue on the FedAuth side (CMS-6976)
                string singleEncodedURL = HttpContext.Current.Server.UrlEncode(HttpContext.Current.Request.RawUrl.ToLower());
                string doubleEncodedURL = HttpContext.Current.Server.UrlEncode(singleEncodedURL);
                HttpContext.Current.Response.Redirect(pathLogin + "?nextPage=" + doubleEncodedURL + defaultProvider);

                return false;
            }

            return false;
        }

        /// <summary>
        /// Validate SSO profile type
        /// </summary>
        public static bool isValidProfileType(string userProfileType)
        {
            bool isProfileTypeValid = false;
            userProfileType = userProfileType.ToLower();

            string SSOValidProfileTypes = ConfigurationManager.AppSettings["SSOValidProfileTypes"] ?? "";
            SSOValidProfileTypes = SSOValidProfileTypes.ToLower() + ",";

            if (userProfileType != "")
            {
                if (SSOValidProfileTypes.Contains(userProfileType + ","))
                {
                    isProfileTypeValid = true;
                }
            }

            return isProfileTypeValid;


        }

    }
}