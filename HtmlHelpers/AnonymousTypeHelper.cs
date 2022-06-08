using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MVC.Extensions.HtmlHelpers
{
    public static class AnonymousTypeHelper
    {
        public static dynamic MergeAnonymousTypes(this HtmlHelper html, params object[] items)
        {
            return MergeAnonymousTypes(items);
        }

        public static dynamic MergeAnonymousTypes(params object[] items)
        {
            object expando = new ExpandoObject();
            var res = expando as IDictionary<string, object>;
            if (items != null)
            {
                foreach(var itm in items)
                {
                    if (itm == null) continue;
                    foreach(PropertyInfo fi in itm.GetType().GetProperties())
                    {
                        if (res.ContainsKey(fi.Name)) continue;
                        res[fi.Name] = fi.GetValue(itm, null);
                    }
                }
            }
            return res;
        }
    }
}
