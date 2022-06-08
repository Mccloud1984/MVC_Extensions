using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC.Extensions.AccessManager
{
    public abstract class ReportsEntities : DbContext
    {

        public ReportsEntities(string name) : base(name) { }

        public virtual DbSet<Report> Reports { get; set; }

        public virtual  DbSet<ReportsRight> ReportsRights { get; set; }


        public Report GetReport(string reportName)
        {
            return Reports.Where(x => x.Name.Equals(reportName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
        }
    }
}
