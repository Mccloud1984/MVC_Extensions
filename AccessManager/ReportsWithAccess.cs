using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;

namespace MVC.Extensions.AccessManager
{
    public class ReportsWithAccess
    {
        public IReport Report { get;}

        public bool HasAccess { get; set; }
        public bool CanEdit { get; set; }
        public IRight AssignedRights { get; set; }

        public ReportsWithAccess(IReport report)
        {
            Report = report;
            HasAccess = false;
            CanEdit = false;
             
        }

        internal void UpdateAccess(IPrincipal principal, string spoofedUser = null)
        {
            UpdateAccess(principal, Report?.ReportsRights.ToArray(), spoofedUser);
        }

        internal void UpdateAccess(IPrincipal principal, IEnumerable<IRight> reportsRight, string spoofedUser = null)
        {
            if (reportsRight == null || Report == null)
                return;
            string sAMAccountName = spoofedUser ?? principal?.Identity?.Name;
            if(String.IsNullOrWhiteSpace(sAMAccountName)) return;
            if (Report.Public) HasAccess = true;
            PrincipalContext context = new PrincipalContext(ContextType.Domain, "CORP");
            UserPrincipal userPrin = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, sAMAccountName);
            var foundRights = (from r in reportsRight
                               where r.Report.ReportId == Report.ReportId
                               select r).ToList();
            foreach (IRight right in foundRights)
            {
                if(Report.ReportId == right.Report.ReportId)
                {
                    if (userPrin.SamAccountName == right.sAMAccountName)
                    {
                        HasAccess = true;
                        CanEdit = right.CanEdit;
                        AssignedRights = right;
                        if (CanEdit) break;
                    }
                    try
                    {
                        if (right.IsGroup)
                        {
                            if (userPrin.IsMemberOf(context, IdentityType.SamAccountName, right.sAMAccountName))
                            {
                                HasAccess = true;
                                CanEdit = right.CanEdit;
                                AssignedRights = right;
                                if (CanEdit) break;
                            }
                        }
                    }
                    catch (Exception)
                    { }
                }
            }
        }
    }
}