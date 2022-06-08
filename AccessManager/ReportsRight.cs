using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC.Extensions.AccessManager
{

    public class ReportsRight : IRight
    {
        [Key]
        public int RightsId { get; set; }
        public string sAMAccountName { get; set; }
        public int Reports_ReportId { get; set; }
        public bool CanEdit { get; set; }
        public bool IsGroup { get; set; }
        [ForeignKey("Reports_ReportId")]
        public virtual Report Report { get; set; }
    }
}
