using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MVC.Extensions.HtmlHelpers
{
    public static class TagBuilderHelper
    {
        public static void MergeAttributes(this TagBuilder tag, object htmlAttributes)
        {
            if(htmlAttributes != null)
            {
                var htmlAttributesDict = HtmlHelper.ObjectToDictionary(htmlAttributes);
                tag.MergeAttributes(htmlAttributesDict, true);
            }
        }
    }
}
