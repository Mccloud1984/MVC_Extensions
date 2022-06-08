using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel
{
    public class HideAttribute : Attribute
    {
        public bool ShowTable { get; set; }
        public bool ShowList { get; set; }
        public bool ShowExport { get; set; }
        public HideAttribute()
        {
            //ShowTable = showTable;
            //ShowList = showList;
        }
    }
}
