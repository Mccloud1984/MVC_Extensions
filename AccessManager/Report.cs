using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC.Extensions.AccessManager
{ 
    public class Report : PageData, IReport
    {
        public Report()
        {
            this.ReportsRights = new HashSet<ReportsRight>();
        }
        [Key]
        public int ReportId { get; set; }
        public bool Public { get; set; }
        public string Category { get; set; }
        public Nullable<bool> ShowInReports { get; set; }
        public string DisplayName { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Name { get; set; }
        [ForeignKey("Reports_ReportId")]
        public virtual ICollection<ReportsRight> ReportsRights { get; set; }

    }
}
