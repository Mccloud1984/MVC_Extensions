using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC.Extensions
{
    public interface IPageSettings
    {

        int GetDefaultPageSize();
        int GetDefaultPageNumber();

        string GetIndexPageName();

        string GetShortName();

        bool CreatePageEnabled { get;  }
        string GetCreatePageName();

        bool EditPageEnabled { get; }
        string GetEditPageName();

        bool SizePageEnabled { get; }
        string GetSizePageName();

        bool ExportPageEnabled { get; }
        string GetExportPageName();

        bool DetailsPageEnabled { get; }
        string GetDetailsPageName();

        bool ClearPageEnabled { get; }
        string GetClearPageName();

        bool FilterPageEnabled { get; }
        string GetFilterPageName();

        bool DeletePageEnabled { get; }
        string GetDeletePageName();

        bool SortPageEnabled { get; }
        IEnumerable<int> GetPageSizeList();

        string GetSortPageName();

        string GetLoadFilterPageName();

        string GetSaveFilterPage();
    }
}
