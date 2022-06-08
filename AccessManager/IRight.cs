using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC.Extensions.AccessManager
{
    public interface IRight
    {
        int RightsId { get; set; }
        string sAMAccountName { get; set; }
        int Reports_ReportId { get; set; }
        bool CanEdit { get; set; }
        bool IsGroup { get; set; }

        Report Report { get; }
    }
}
