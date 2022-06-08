using CMWME.C_Extensions;
using X.PagedList;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using CMWME.LogLib;
using MVC.Extensions.AccessManager;

namespace MVC.Extensions.PageBuilder
{

    public class PageSchema<TModel> : IDisposable, IPageSchema where TModel: PageData
    {
        
        public double TimerTickTime = TimeSpan.FromHours(1).TotalMilliseconds;

        internal Task<List<TModel>> GetDBList()
        {
            CheckDbContext();
            return dbContext.Set<TModel>()?.ToListAsync();
        }


        public void SetSessionValue(string name, object value)
        {
            try
            {
                MVCVariables.GetSession()[name] = value;
            }
            catch (Exception ex)
            {
                LogFx.AddError($"Failed to set seeion value of {name} to {value}.", innerException: ex);
            }
        }

        public object GetSessionValue(string name) => MVCVariables.GetSession()?[name];

        public void ClearSessionValue(string name) => SetSessionValue(name, null);

        public string ReportName { get; }
        public IPageSettings Settings { get; }
        public Type ModelType => typeof(TModel);
        public PropertyInfo[] ModelProperties { get; }


        public IEnumerable<IPageFilter> BaseFilters { get; private set; }
        public IEnumerable<string> Reports { get; private set; }
        public IEnumerable BaseModel { get; private set; }

        private IReport reportsAccess { get; set; }

        private DbContext dbContext { get; set; }

        public Timer CacheTimer { get; private set; }

        public PageSchema(DbContext dbContext, IReport reportsAccess)
            : this(dbContext, reportsAccess, null)
        {
        }
        public PageSchema(DbContext dbContext = null, IReport reportsAccess = null, PageSettings settings = null)
        {  
            Settings = settings ?? new PageSettings();
            this.dbContext = dbContext;
            this.reportsAccess = reportsAccess;
            ModelProperties = ModelType.GetProperties();
            CacheTimer = new Timer(TimerTickTime);
            CacheTimer.Elapsed += CacheTimer_Elapsed;
            CacheTimer.Start();
            PageStore.AddNewPage(this);
        }

        public void ClearCache()
        {
            Reports = null;
            BaseModel = null;
            BaseFilters = null;
        }

        public IEnumerable<TModel> GetModel() => BaseModel == null ? null : (IEnumerable<TModel>)BaseModel;
        public int GetModelCount() => GetModel()?.GetCount() ?? 0;

        private void CacheTimer_Elapsed(object sender, ElapsedEventArgs e) => ClearSession();

        public async Task SetBaseFilters(IEnumerable<IPageFilter> filters)
        {
            await Task.Run(() =>
            {
                BaseFilters = filters;
                List<string> reports = new List<string>() { "" };
                reports.AddRange(filters.Select(x => x.Report).Distinct());
                Reports = reports;
            });
        }

        public bool CreateClearControl() => Settings.ClearPageEnabled;
        
        public string GetClearPageURL(UrlHelper url)
        {
            return url.Action(Settings.GetClearPageName(), MVCVariables.GetRequestControllerName());
        }

        public bool CreateDetailsControl() => Settings.DetailsPageEnabled;
        public string GetDetailsPageURL(UrlHelper url, object id)
        {
            return url.Action(Settings.GetDetailsPageName(), MVCVariables.GetRequestControllerName(), new { id });
        }

        public string GetFilterPageURL(UrlHelper url) => url.Action(Settings.GetFilterPageName(), MVCVariables.GetRequestControllerName());

        public string GetFilterControl(HtmlHelper helper, IPageFilter filter, int index, bool textArea) => helper.GetFilterControl(this, filter, index, textArea).ToString();

        public bool CreateEditControl() => CanEdit(MVCVariables.GetUserPrin()) && Settings.EditPageEnabled;

        public bool CreateDeleteControl() => CanEdit(MVCVariables.GetUserPrin()) && Settings.DeletePageEnabled;

        public bool CreateCreateControl() => CanEdit(MVCVariables.GetUserPrin()) && Settings.CreatePageEnabled;

        public bool CreateExportControl() => Settings.ExportPageEnabled;

        public bool CreateSortControl() => Settings.SortPageEnabled;

        public bool CreateSaveControl() => !string.IsNullOrWhiteSpace(Settings.GetSaveFilterPage());

        public bool CreateLoadControl() => !string.IsNullOrWhiteSpace(Settings.GetLoadFilterPageName());

        public bool CreatePageSizeControl() => Settings.SizePageEnabled;

        public bool Initialized() => GetModelCount() > 0;

        public string GetCurrentSort() => GetSessionValue(GetCurrentSortName())as string;

        public string GetCurrentSortName() =>$"{Settings.GetShortName()}_CurrentSort";

        public string GetCurrentPageSizeName() => $"{Settings.GetShortName()}_CurrentPageSize";

        public object GetCurrentQuery() => GetSessionValue(GetCurrentQueryName());

        public string GetCurrentQueryName() =>$"{Settings.GetShortName()}_CurrentQuery";

        public void SetPropertySortParam(PropertyInfo prop)
        {
            string currentSort = GetCurrentSort();
            string sessVal = currentSort == prop.Name ? GetNameDesc(prop) : currentSort == GetNameDesc(prop) ? GetNameAsc(prop) : prop.Name;
            SetSessionValue(GetPropertySortPramName(prop), sessVal);
        }

        public string GetPropertySortPramName(PropertyInfo prop) =>$"{Settings.GetShortName()}_{prop?.Name}_Param";
        public void SetCurrentQuery(IEnumerable<IPageFilter> query)
        {
            SetSessionValue(GetCurrentQueryName(), query ?? GetSessionValue(GetCurrentQueryName()));
            SetCurrentFilterIndex(query?.Count() ?? 1 - 1);
        }

        public void SetCurrentQuery(string query)
        {
            SetSessionValue(GetCurrentQueryName(), query  ?? GetSessionValue(GetCurrentQueryName()));
        }

        public void AddEmptyQueryItem()
        {
            List<IPageFilter> filters = new List<IPageFilter>();
            IEnumerable<IPageFilter> cFilters = ((IEnumerable<IPageFilter>)GetSessionValue(GetCurrentQueryName()));
            if (cFilters != null)
            {
                foreach (var item in cFilters)
                {
                    filters.Add(item);
                }
            }
            filters.Add(new PageFilter());
            if (filters.Count <= 1) filters.Add(new PageFilter());
            SetSessionValue(GetCurrentQueryName(), filters);
        }

        public int GetCurrentFilterIndex() => GetSessionValue(GetCurrentFilterIndexName()) as int? ?? 0;

        public void SetCurrentFilterIndex(int? index)
        {

            SetSessionValue(GetCurrentFilterIndexName(), index ?? 0);
        }

        public string GetCurrentFilterIndexName() => $"{Settings.GetShortName()}_CurrentFilterCount";


        public void SetCurrentSort(string sortOrder) => SetSessionValue(GetCurrentSortName(), sortOrder ?? GetSessionValue(GetCurrentSortName()));

        public object GetPropertySortPram(PropertyInfo mp) =>GetSessionValue(GetPropertySortPramName(mp)) ?? GetNameDesc(mp);

        public void ClearSession()
        {
            ClearCurrentQuery();
            ClearCurrentSort();
        }

        private void ClearCurrentSort() => ClearSessionValue(GetCurrentQueryName());

        private void ClearCurrentQuery() => ClearSessionValue(GetCurrentSortName());

        public UrlHelper GetCurrentUrl() => new UrlHelper(MVCVariables.GetCurrentRequest().RequestContext);

        public void SetCurrentPageSize(int? pageSize) => SetSessionValue(GetCurrentPageSizeName(), pageSize ?? GetCurrentPageSize());

        public int GetCurrentPageSize() => GetSessionValue(GetCurrentPageSizeName()) as int? ?? Settings.GetDefaultPageSize();

        public string GetCurrentPageNumberName() => $"{Settings.GetShortName()}_CurrentPageNumber";

        public void SetCurrentPageNumber(int? page) => SetSessionValue(GetCurrentPageNumberName(), page ?? GetCurrentPageNumber());

        public int GetCurrentPageNumber() => GetSessionValue(GetCurrentPageNumberName()) as int? ?? Settings.GetDefaultPageNumber();

        public string GetCurrentTotalItemCountName() => $"{Settings.GetShortName()}_CurrentTotalItems"; 

        public int GetCurrentTotalItemCount() => GetSessionValue(GetCurrentTotalItemCountName()) as int? ?? GetModelCount(); 

        public void SetCurrentTotalItemCount(int? totalItems) => SetSessionValue(GetCurrentTotalItemCountName(), totalItems ?? GetCurrentTotalItemCount());

        public string GetCurrentPageCountName() => $"{Settings.GetShortName()}_PageCount";

        public int? GetCurrentPageCount() => GetSessionValue(GetCurrentPageCountName()) as int?;

        public void SetCurrentPageCount(int? pageCount) => SetSessionValue(GetCurrentPageCountName(), pageCount);

        public bool CanEdit() => CanEdit(MVCVariables.GetUserPrin());


        public bool CanEdit(IPrincipal user) => ReportsWithAccessManager.CanEditReport(reportsAccess, user);
        
        public IEnumerable GetFilterdList(IEnumerable model = null)
        {
            model = model ?? GetModel();
            if (model == null) return null;
            object currentQuery = GetCurrentQuery();
            if (currentQuery != null)
            {
                if (currentQuery.IsType(out string currentQueryString))
                    model = ((IEnumerable<TModel>)model)?.QueryAll(currentQueryString);
                else
                    model = ((IEnumerable<TModel>)model)?.QueryAll(currentQuery as IEnumerable<IPageFilter>);
            }
            model = SortModel(model);
            return model;
        }

        public const string DESCPOSTFIX = "_desc";
        public const string ASCPOSTFIX = "_asc";

        public IEnumerable<TModel> SortModel(object model = null) => SortModel((IEnumerable<TModel>)model);


        public IEnumerable<TModel> SortModel(IEnumerable<TModel> model = null)
        {
            string sortOrder = GetCurrentSort();
            if(model == null) return model;
            bool foundProp = false;
            if (!string.IsNullOrWhiteSpace(sortOrder))
            {
                foreach (var prop in ModelProperties)
                {
                    //PropertyInfo prop = ModelProperties[i];
                    SetPropertySortParam(prop);
                    if (GetNameAsc(prop) == sortOrder)
                    {
                        model = model.OrderBy(x => prop.GetValue(x));
                        foundProp = true;
                    }
                    else if (GetNameDesc(prop) == sortOrder)
                    {
                        model = model.OrderByDescending(x => prop.GetValue(x));
                        foundProp = true;
                    }
                }
            }
            if(!foundProp)
                model = model.OrderBy(x => ModelProperties[0].GetValue(x));
            return model;
        }


        public string GetNameDesc(PropertyInfo prop)
        {
            return $"{prop.Name}{DESCPOSTFIX}";
        }
        public string GetNameAsc(PropertyInfo prop)
        {
            return $"{prop.Name}{ASCPOSTFIX}";
        }
        private void CheckDbContext() => dbContext = dbContext ?? throw new DbContextNullException(); 
        public Task AddModelAsync(TModel model)
        {
            CheckDbContext();
            dbContext.Entry(model).State = EntityState.Added;
            return dbContext.SaveChangesAsync();
        }
        public Task SaveModelAsync(TModel model)
        {
            CheckDbContext();
            dbContext.Entry(model).State = EntityState.Modified;
            return dbContext.SaveChangesAsync();
        }


        public Task DeleteModelAsync(TModel model) 
        {
            CheckDbContext();
            dbContext.Entry(model).State = EntityState.Deleted;
            return dbContext.SaveChangesAsync();
        }

        public async Task<IPagedList> GetPagedListAsync(IEnumerable model = null, int? page = null)
        {
            SetCurrentPageNumber(page);
            //Model = model ?? Model ?? BaseModel;
            model = model ?? GetModel();
            if (model == null) return null;

            model = GetFilterdList(model);

            IPagedList<TModel> retModel = await ((IEnumerable<TModel>)model).ToPagedListAsync(GetCurrentPageNumber(), GetCurrentPageSize());
            SetCurrentPageSize(retModel.PageSize);
            SetCurrentTotalItemCount(retModel.TotalItemCount);
            SetCurrentPageCount(retModel.PageCount);
            SetCurrentPageNumber(retModel.PageNumber);
            return retModel;
        }

        public Task<TModel> GetModelById(object id)
        {
            CheckDbContext();
            object cId = castAsId(id);
            return dbContext.Set<TModel>()?.FindAsync(cId);
        }

        private object castAsId(object id)
        {
            string idStr = id.ToString();
            if(int.TryParse(idStr, out int intResult))
            {
                return intResult;
            }
            else
            {
                return idStr;
            }
        }

        public void ClearBaseModel() => BaseModel = null;

        public void Dispose()
        {
            Dispose(true);
        }

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                ((IDisposable)CacheTimer).DisposeIfNotNull();
                dbContext?.DisposeIfNotNull();
            }
            dbContext = null;
            CacheTimer = null;
            BaseFilters = null;
            Reports = null;
            BaseModel = null;
            reportsAccess = null;
        }

        public void SetBaseModel(IEnumerable baseModel = null) => BaseModel = baseModel;

        public async Task<FileContentResult> ToCSV(object model = null)
        {
            IEnumerable query = null;
            if (model != null)
                query = (IEnumerable)model;
            else
                query = GetFilterdList();
            string csv = await query.ToCSVAsync<TModel>();
            string fileName = $"{Settings.GetShortName()}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.csv";
            //HttpContext.Response.AddHeader("content-disposition", $"attachment; filename={fileName}");
            var file = 
                new FileContentResult(new UTF8Encoding().GetBytes(csv), "text/csv")
                {
                    FileDownloadName = fileName
                };
            return file;
        }

        public void SetSinglePageQuery() => SetCurrentPageSize(GetModelCount() + 1);

        public async Task CheckandInitializeAsync()
        {
            if (!Initialized())
            {
                await InitializeAsync();
            }
        }

        public async Task InitializeAsync()
        {
            CheckDbContext();
            SetBaseModel(await dbContext?.Set<TModel>()?.ToListAsync());
        }

        ~PageSchema()
        {
            Dispose(false);
        }
    }


    [Serializable]
    public class DbContextNullException : Exception
    {
        public DbContextNullException() { }
    }
}