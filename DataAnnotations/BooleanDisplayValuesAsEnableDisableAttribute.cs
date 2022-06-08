using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC.Extensions.DataAnnotations
{
    /// <summary>
    /// For a boolean field, set the display text for <c>true</c> and
    /// <c>false</c> values to "Enable" and "Disable".
    /// </summary>
    public class BooleanDisplayValuesAsEnableDisableAttribute :
        BooleanDisplayValuesAttribute
    {
        public BooleanDisplayValuesAsEnableDisableAttribute()
            : base("Enable", "Disable")
        {
        }
    }

}
