using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC.Extensions
{
    public interface IReportsContext
    {

        IEnumerable<IReport> Reports { get; }
    }
}
