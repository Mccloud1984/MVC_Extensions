using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC.Extensions
{
    public class ModelInfo
    {
        public object Model { get; }
        public string CurrentColumn { get; }

        public ModelInfo(object model, string currentColumn)
        {
            Model = model;
            CurrentColumn = currentColumn;
        }
    }
}
