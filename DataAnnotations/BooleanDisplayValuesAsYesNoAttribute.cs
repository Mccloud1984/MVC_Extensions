using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC.Extensions.DataAnnotations
{
    /// <summary>
    /// For a boolean field, set the display text for <c>true</c> and
    /// <c>false</c> values to "Yes" and "No".
    /// </summary>
    public class BooleanDisplayValuesAsYesNoAttribute :
        BooleanDisplayValuesAttribute
    {
        public BooleanDisplayValuesAsYesNoAttribute()
            : base("Yes", "No")
        {
        }
    }

}
