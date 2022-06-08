using CMWME.LogLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;

namespace MVC.Extensions
{
    public enum RunMode
    {
        Prod,
        Test,
        Dev
    }
    public static class MVCVariables
    {
        public static string GetFQDN()
        {
            string fqdn = "";
            try
            {
                fqdn = GetCurrentRequest()?.UserHostAddress;
                fqdn = Dns.GetHostEntry(fqdn)?.HostName ?? "";
            }
            catch (Exception ex)
            {
                LogFx.AddError("Failed to get fqdn.", innerException: ex);
            }
            return fqdn ?? "";
        }

        private static string[] GetComputerParts()
        {
            try
            {
                return GetFQDN()?.Split(new char[] { '.' }, 2, StringSplitOptions.RemoveEmptyEntries);
            }
            catch (Exception ex)
            {
                LogFx.AddError("Failed to get computer parts.", innerException: ex);
                return new string[0];
            }
        }

        public static string GetComputerName()
        {
            try
            {
                var compParts = GetComputerParts();
                return (compParts?.Length > 0 ? compParts[0] : "") ?? "";
            }
            catch (Exception ex)
            {
                LogFx.AddError("Failed to get computer name.", innerException: ex);
                return "";
            }
        }

        public static string GetComputerDomain()
        {
            try
            {
                string[] compParts = GetComputerParts();
                return (compParts.Length > 1 ? compParts[1] : "") ?? "";
            }
            catch (Exception ex)
            {
                LogFx.AddError("Failed to get computer domain.", innerException: ex);
                return "";
            }
        }

        public static string SiteHostName => $"{GetFullRootUrl()}/";

        private static string GetFullRootUrl()
        {
            try
            {
                return GetUrl()?.Replace(GetCurrentRequest().Url?.AbsolutePath, String.Empty) ?? "";
            }
            catch (Exception ex)
            {
                LogFx.AddError("Failed to get full root url.", innerException: ex);
                return "";
            }
        }

        public static string IISServer => Environment.MachineName;

        public static string GetBrowser()
        {
            try
            {
                return GetCurrentRequest()?.Browser?.Browser;
            }
            catch (Exception ex)
            {
                LogFx.AddError("Failed to get browser.", innerException: ex);
                return "";
            }
        }

        public static string GetBrowserVersion()
        {
            try
            {
                return GetCurrentRequest()?.Browser?.Version;
            }
            catch (Exception ex)
            {
                LogFx.AddError("Failed to get browser version.", innerException: ex);
                return "";
            }
        }

        public static string GetUrl()
        {
            try
            {
                return GetCurrentRequest()?.Url?.AbsoluteUri;
            }
            catch (Exception ex)
            {
                LogFx.AddError("Failed to get url.", innerException: ex);
                return "";
            }
        }

        public static string GetRequestControllerName()
        {
            return GetCurrentContext()?.GetRequestControllerName();
        }

        public static string GetRequestActionName()
        {
            return GetCurrentContext()?.GetRequestActionName();
        }

        public static string GetRequestId()
        {
            return GetCurrentContext().GetRequestId();
        }

        public static HttpSessionState GetSession()
        {
            try
            {
                return GetCurrentContext()?.Session;
            }
            catch (Exception ex)
            {
                LogFx.AddError("Failed to get current sesion.", innerException: ex);
                return null;
            }
        }

        public static HttpContext GetCurrentContext()
        {
            try
            {
                return HttpContext.Current;
            }
            catch (Exception ex)
            {
                LogFx.AddError("Failed to get current http context.", innerException: ex);
                return null;
            }
        }

        public static HttpRequest GetCurrentRequest()
        {
            try
            {
                return GetCurrentContext()?.Request;
            }
            catch (Exception ex)
            {
                LogFx.AddError("Failed to get current request context.", innerException: ex);
                return null;
            }
        }

        public static string GetRequestType()
        {
            return GetCurrentRequest()?.RequestType;
        }

        private static string _userId = null;
        public static string GetUserId()
        {
            _userId = GetUserPrin()?.Identity?.Name ?? _userId;
            return _userId;
        }

        private static IPrincipal _user = null;
        public static IPrincipal GetUserPrin()
        {
            _user = GetCurrentContext()?.User ?? _user;
            return _user;
        }

        public static RunMode GetRunMode()
        {
            RunMode mode = RunMode.Prod;
            string url = GetUrl();
            if (url.IndexOf("-d/", StringComparison.CurrentCultureIgnoreCase) > 1 || url.IndexOf("-d?", StringComparison.CurrentCultureIgnoreCase) > 1)
            {
                mode = RunMode.Dev;
            }
            else if (url.IndexOf("localhost", StringComparison.CurrentCultureIgnoreCase) > 1)
            {
                mode = RunMode.Dev;
            }
            else if (url.IndexOf("-t/", StringComparison.CurrentCultureIgnoreCase) > 1 || url.IndexOf("-t?", StringComparison.CurrentCultureIgnoreCase) > 1)
            {
                mode = RunMode.Test;
            }
            return mode;
        }
    }
}
