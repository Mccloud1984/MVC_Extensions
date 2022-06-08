using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace MVC.Extensions.AccessManager
{
    public static class ReportsWithAccessManager
    {

        internal static IPrincipal _principal = null;

        internal static string spoofedUser = null;

        internal static IEnumerable<IReport> _reportsAccess = null;

        private static ReportsWithAccessCollection _reports = null;

        public static void SetSpoofUser(IEnumerable<IReport> reports, IPrincipal principal, string spoofUser)
        {
            spoofedUser = spoofUser;
            _reports = ReportsWithAccessCollection.GenerateCollection(reports, principal, spoofedUser);
        }

        public static ReportsWithAccessCollection GetReportsWithAccess(IEnumerable<IReport> reports, IPrincipal principal)
        {
            if (_principal != principal)
                spoofedUser = null;
            if (_reports == null || _principal != principal)
                _reports = ReportsWithAccessCollection.GenerateCollection(reports, principal);
            _principal = principal;
            return _reports;
        }

        public static bool CanEditReport(string reportName)
        {
            if ((_reports?.Count() ?? 0) <= 0) return false; // throw new ArgumentNullException("Reports not populated.");
            ReportsWithAccess report = _reports?[reportName];
            return CanEditReport(report);
        }

        public static bool CanEditReport(IReport report, IPrincipal principal)
        {
            var rpt = new ReportsWithAccess(report);
            rpt.UpdateAccess(principal);
            return CanEditReport(rpt);
        }

        public static bool CanEditReport(IEnumerable<IReport> reports, IPrincipal principal, string reportName)
        {
            GetReportsWithAccess(reports, principal);
            ReportsWithAccess report = _reports?[reportName];
            return CanEditReport(report);
        }


        public static bool CanEditReport(ReportsWithAccess report)
        {
            return report?.CanEdit ?? false;
        }
    }
}