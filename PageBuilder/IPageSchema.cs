using X.PagedList;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MVC.Extensions
{
    public interface IPageSchema
    {
        IPageSettings Settings { get; }

        void ClearCache();

        IEnumerable<String> Reports { get; }

        Type ModelType { get; }


        PropertyInfo[] ModelProperties { get; }
        
        UrlHelper GetCurrentUrl();
        object GetCurrentQuery();
        bool CreateCreateControl();
        bool CreatePageSizeControl();
        bool CreateExportControl();
        bool CreateSaveControl();
        bool CreateLoadControl();
        int GetCurrentTotalItemCount();
        int GetCurrentPageSize();
        string GetFilterPageURL(UrlHelper url);
        string GetFilterControl(HtmlHelper helper, IPageFilter filter, int index, bool textArea);
        int GetCurrentFilterIndex();
        void SetCurrentFilterIndex(int? index);
        bool CreateClearControl();
        string GetCurrentSort();
        void SetCurrentSort(string sortOrder);
        bool CreateEditControl();
        bool CreateDetailsControl();
        bool CreateDeleteControl();
        bool CreateSortControl();
        object GetPropertySortPram(PropertyInfo prop);
        void SetCurrentQuery(IEnumerable<IPageFilter> query);
        void SetCurrentQuery(string query);
        Task<IPagedList> GetPagedListAsync(IEnumerable model = null, int? page = null);
        void SetBaseModel(IEnumerable baseModel = null);
        Task<FileContentResult> ToCSV(object model = null);
        bool Initialized();

        void ClearSession();
        string GetDetailsPageURL(UrlHelper url, object id);
        //void SetBaseModel(object query);

        string GetClearPageURL(UrlHelper url);
        Task CheckandInitializeAsync();

        Task InitializeAsync();
        void SetCurrentPageNumber(int? page);
        void SetCurrentPageSize(int? pageSize);

        string GetNameDesc(PropertyInfo prop);
        string GetNameAsc(PropertyInfo prop);
    }
}
