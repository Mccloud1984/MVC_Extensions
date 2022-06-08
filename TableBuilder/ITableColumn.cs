using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC.Extensions.Table
{
    public interface ITableColumn
    {
        void SetTemplate();
        void SetTemplate(string templateName);
    }
}
