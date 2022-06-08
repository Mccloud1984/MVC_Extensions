using System;
using System.Linq;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Data.Entity;

namespace MVC.Extensions.AccessManager
{

    public class ReportsWithAccessCollection : List<ReportsWithAccess>
    {
        //private DbSet _reportsDB = null;
        //private DbSet _rightsDB = null;

        public ReportsWithAccess this[string Name]
        {
            get
            {
                for (int i = 0; i < this.Count; i++)
                {
                    ReportsWithAccess rptAcc = this[i];
                    if (rptAcc.Report.Name == Name)
                        return rptAcc;
                }
                return null;
            }
        }

        internal static ReportsWithAccessCollection GenerateCollection(IEnumerable<IReport> reports, IPrincipal principal, string spoofedUser = null)
        {
            return new ReportsWithAccessCollection(reports, principal, spoofedUser);
        }
        
        private ReportsWithAccessCollection(IEnumerable<IReport> reports, IPrincipal principal, string spoofedUser = null)
        {
            if(reports?.Count() > 0)
            {
                List<ReportsWithAccess> tList = new List<ReportsWithAccess>();
                foreach(IReport rpt in reports)
                {
                    ReportsWithAccess rptAcc = new ReportsWithAccess(rpt);
                    rptAcc.UpdateAccess(principal, spoofedUser);
                    tList.Add(rptAcc);
                }
                this.AddRange(tList.OrderBy(x => x.Report.DisplayName).OrderBy(x => x.Report.Category));
            }
        }

    }

}