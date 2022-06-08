using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC.Extensions
{
    public static class PageStore
    {
        private static List<IPageSchema> _pageStore = new List<IPageSchema>();

        internal static void AddNewPage(IPageSchema page)
        {
            _pageStore = _pageStore ?? new List<IPageSchema>();
            _pageStore.Add(page);
        }

        //public static IPageSchema GetPage(string pageName)
        //{
        //    foreach (var item in _pageStore)
        //    {
        //        if(item.Settings.GetIndexPageName()?.Equals(pageName, StringComparison.CurrentCultureIgnoreCase) == true)
        //        {
        //            return item;
        //        }
        //    }
        //    return null;
        //}

        public static void ClearPageStore()
        {
            foreach (var item in _pageStore)
            {
                item.ClearCache();
            }
        }
    }
}
