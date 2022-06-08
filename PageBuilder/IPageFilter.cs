using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC.Extensions
{
    public interface IPageFilter
    {
        int FilterId { get; }
        string Report { get; }
        string Field { get; }
        string Condition { get; }
        string Value { get; }
    }
}
