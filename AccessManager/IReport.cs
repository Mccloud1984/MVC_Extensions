using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC.Extensions.AccessManager
{
    public interface IReport
    {

        int ReportId { get; set; }
        bool Public { get; set; }
        string Category { get; set; }
        Nullable<bool> ShowInReports { get; set; }
        string DisplayName { get; set; }
        string Controller { get; set; }
        string Action { get; set; }
        string Name { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        ICollection<ReportsRight> ReportsRights { get; }
    }
}
