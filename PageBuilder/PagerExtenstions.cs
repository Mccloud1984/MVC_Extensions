using X.PagedList;
using X.PagedList.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using X.PagedList.Mvc.Common;

namespace MVC.Extensions
{
    internal static class PagerExtenstions
    {
        public static System.Web.HtmlString PagedListPager(System.Web.Mvc.HtmlHelper html, IPagedList list, Func<int, string> generatePageUrl)
        {
            return html.PagedListPager(list, generatePageUrl, PagedListRenderOptions.ClassicPlusFirstAndLast);
        }

        public static System.Web.HtmlString PagedListPagerLimit5(System.Web.Mvc.HtmlHelper html, IPagedList list, Func<int, string> generatePageUrl)
        {
            return html.PagedListPager(list, generatePageUrl, PagedListRenderOptions.OnlyShowFivePagesAtATime);
        }

        internal static IEnumerable<int> GetPageList(IPagedList model)
        {
            List<int> retList = new List<int>();
            for (int i = 1; i <= model.PageCount; i++)
            {
                retList.Add(i);
            }
            return retList;
        }
    }
}
