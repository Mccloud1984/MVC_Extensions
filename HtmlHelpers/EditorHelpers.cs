using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MVC.Extensions.HtmlHelpers
{
    public static class EditorHelpers
    {
        public static MvcHtmlString EnumDropDownList(this HtmlHelper html, Type enumType, int? currentValue = null, object htmlAttributes = null)
        {
            
            TagBuilder selectTag = new TagBuilder("select");
            selectTag.MergeAttributes(htmlAttributes);
            IEnumerable<string> enumArray = Enum.GetNames(enumType);
            foreach (var enumName in enumArray)
            {
                TagBuilder enumTag = new TagBuilder("option");
                int enumVal = Convert.ToInt32(Enum.Parse(enumType, enumName));
                enumTag.MergeAttribute("Value", enumVal.ToString());
                if(enumVal == currentValue || (enumVal == 0 && currentValue == null))
                {
                    enumTag.MergeAttribute("Selected", "Selected");
                }
                enumTag.InnerHtml = enumName;
                selectTag.InnerHtml += enumTag.ToString();
            }
            return MvcHtmlString.Create(selectTag.ToString());
        }
    }
}
