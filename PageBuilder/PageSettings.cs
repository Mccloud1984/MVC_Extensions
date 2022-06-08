using MVC.Extensions.PageBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC.Extensions.PageBuilder
{
    public class PageSettings : IPageSettings
    {
        const string DEFAULTDETAILSPAGE = nameof(IPageController.Details);

        const string DEFAULTCLEARPAGE = nameof(IPageController.Clear);


        const string DEFAULTFILTERPAGE = nameof(IPageController.Filter);

        const string DEFAULTEDITPAGE = nameof(IPageController.Edit);

        const string DEFAULTEXPORTPAGE = nameof(IPageController.Export);

        const string DEFAULTSIZEPAGE = nameof(IPageController.PageSize);

        const string DEFAULTSORTPAGE = nameof(IPageController.Sort);

        const string DEFAULTDELETEPAGE = nameof(IPageController.Delete);

        const string DEFAULTCREATEPAGE = nameof(IPageController.Create);

        const string DEFAULTINDEXPAGE = nameof(IPageController.Index);
        readonly string DEFAULTSHORTNAME = $"Generic_{DateTime.Now.ToString("yyMMdd_HH")}";

        public int DefaultPageSize = 200;
        public int GetDefaultPageSize() => DefaultPageSize;

        public int DefaultPageNumber = 1;
        public int GetDefaultPageNumber() => DefaultPageNumber;


        public string GetIndexPageName() => _indexPage ?? DEFAULTINDEXPAGE;
        private string _indexPage;
        public string IndexPage
        {
            set { _indexPage = value; }
        }

        public string GetShortName() => _shortName ?? DEFAULTSHORTNAME;
        private string _shortName;
        public string ShortName { set { _shortName = value; } }

        private string _clearPage;
        public string ClearPage
        {
            set { ClearPageEnabled = true; _clearPage = value; }
        }
        public bool ClearPageEnabled { get; set; }
        public string GetClearPageName() => _clearPage ?? DEFAULTCLEARPAGE;

        private string _detailsPage;
        public string DetailsPage
        {
            set { DetailsPageEnabled = true; _detailsPage = value; }
        }
        public bool DetailsPageEnabled { get; set; }
        public string GetDetailsPageName() => _detailsPage ?? DEFAULTDETAILSPAGE;

        private string _sortPage;
        public string SortPage
        {
            set { SortPageEnabled = true; _sortPage = value; }
        }
        public bool SortPageEnabled { get; set; }
        public string GetSortPageName() => _sortPage ?? DEFAULTSORTPAGE;

        private string _exportPage;
        public string ExportPage
        {
            set { ExportPageEnabled = true; _exportPage = value; }
        }
        public bool ExportPageEnabled { get; set; }
        public string GetExportPageName() => _exportPage ?? DEFAULTEXPORTPAGE;


        public IEnumerable<int> PageSizeList = new int[] { 20, 50, 100, 500, 1000, 2000, 5000 };

        public IEnumerable<int> GetPageSizeList() => PageSizeList;


        private string _sizePage;
        public string SizePage
        {
            set { SizePageEnabled = true; _sizePage = value; }
        }
        public bool SizePageEnabled { get; set; }
        public string GetSizePageName() => _sizePage ?? DEFAULTSIZEPAGE;

        private string _editPage;
        public string EditPage
        {
            set { EditPageEnabled = true; _editPage = value; }
        }
        public bool EditPageEnabled { get; set; }
        public string GetEditPageName() => _editPage ?? DEFAULTEDITPAGE;

        private string _deletePage;
        public string DeletePage
        {
            set { DeletePageEnabled = true; _deletePage = value; }
        }
        public bool DeletePageEnabled { get; set; }
        public string GetDeletePageName() => _deletePage ?? DEFAULTDELETEPAGE;

        private string _createPage;
        public string CreatePage
        {
            set { CreatePageEnabled = true; _createPage = value; }
        }
        public bool CreatePageEnabled { get; set; }
        public string GetCreatePageName() => _createPage ?? DEFAULTCREATEPAGE;

        private string _filterPage;
        public string FilterPage
        {
            set { FilterPageEnabled = true; _filterPage = value; }
        }
        public bool FilterPageEnabled { get; set; }
        public string GetFilterPageName() => _filterPage ?? DEFAULTFILTERPAGE;

        private string _saveFilterPage;
        public string SaveFilterPage
        {
            set { _saveFilterPage = value; }
        }
        //public bool EnableSaveFilterPage { get; set; }
        public string GetSaveFilterPage() => _saveFilterPage;

        private string _loadFilterPage;
        public string LoadFilterPage
        {
            set { _loadFilterPage = value; }
        }
        //public bool EnableLoadFilterPage { get; set; }
        public string GetLoadFilterPageName() => _loadFilterPage;
    }
}
