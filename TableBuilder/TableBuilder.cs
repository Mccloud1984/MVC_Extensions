using X.PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace MVC.Extensions.Table
{

    public class TableBuilder<TModel> where TModel : class
    {
        internal IPagedList<TModel> ModelArray { get; }
        internal TModel Model { get; }
        internal HtmlHelper Html { get; }
        internal object HtmlAttributes { get; }
        internal IPageSchema Page { get; }
        public bool ShowPager { get; }

        /// <summary>
        /// List of table columns to be rendered in the table.
        /// </summary>
        internal IList<ITableColumnInternal<TModel>> TableColumns { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        private TableBuilder()
        {
        }

        internal TableBuilder(HtmlHelper<IPagedList<TModel>> helper, IPagedList<TModel> modelArray, IPageSchema page, object htmlAttributes = null, bool showPager = false)
        {
            ModelArray = modelArray ?? throw new ArgumentNullException(nameof(modelArray));
            Html = helper ?? throw new ArgumentNullException(nameof(helper));
            Page = page ?? throw new ArgumentNullException(nameof(page));
            HtmlAttributes = htmlAttributes;
            ShowPager = showPager;
            TableColumns = new List<ITableColumnInternal<TModel>>();
        }

        internal TableBuilder(HtmlHelper<TModel> helper, TModel model, IPageSchema page, object htmlAttributes = null)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
            Html = helper ?? throw new ArgumentNullException(nameof(helper));
            Page = page ?? throw new ArgumentNullException(nameof(page));
            HtmlAttributes = htmlAttributes;
            TableColumns = new List<ITableColumnInternal<TModel>>();
        }

        /// <summary>
        /// Add an lambda expression as a TableColumn.
        /// </summary>
        /// <typeparam name="TProperty">Model class property to be added as a column.</typeparam>
        /// <param name="expression">Lambda expression identifying a property to be rendered.</param>
        /// <returns>An instance of TableColumn.</returns>
        internal ITableColumn AddColumn<TProperty>(Expression<Func<TModel, TProperty>> expression, object htmlAttributes = null)
        {
            TableColumn<TModel, TProperty> column = new TableColumn<TModel, TProperty>(expression, htmlAttributes);
            this.TableColumns.Add(column);
            return column;
        }

        internal ITableColumnInternal<TModel>GetColumn(string ColumnName)
        {
            for (int i = 0; i < TableColumns.Count; i++)
            {
                var col = TableColumns.ElementAt(i);
                if(col.ColumnName == ColumnName)
                {
                    return col;
                }
            }
            return null;
        }
        

        /// <summary>
        /// Create an instance of the ColumnBuilder to add columns to the table.
        /// </summary>
        /// <param name="columnBuilder">Delegate to create an instance of ColumnBuilder.</param>
        /// <returns>An instance of TableBuilder.</returns>
        public TableBuilder<TModel> Columns(Action<ColumnBuilder<TModel>> columnBuilder)
        {
            ColumnBuilder<TModel> builder = new ColumnBuilder<TModel>(this);
            columnBuilder(builder);
            return this;
        }

        /// <summary>
        /// Convert the TableBuilder to HTML.
        /// </summary>
        public IHtmlString ToHtml(bool sortable = true, bool editable = false, string rowHeaderTemplate = null)
        {
            if (ModelArray != null)
                return ToTabularTable(sortable, editable, rowHeaderTemplate);
            else if (Model != null)
                return ToDataListTable(editable);
            else
                throw new Exception("Unknown data table type, missing model.");
        }

        private IHtmlString ToDataListTable(bool editable)
        {
            Type modelType = Page.ModelType;
            PropertyInfo[] modelProps = Page.ModelProperties;
            int colCount = modelProps.Count();
            TagBuilder tableWrapper = new TagBuilder("div");
            tableWrapper.AddCssClass("dl-table-wrapper");
            TagBuilder tableTag = new TagBuilder("table");
            tableTag.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(HtmlAttributes), true);
            tableTag.AddCssClass("table table-hover table-striped table-bordered table-sm table-responsive dl-table");
            for (int i = 0; i < colCount; i++)
            {
                PropertyInfo mp = modelProps.ElementAt(i);
                var column = GetColumn(mp.Name);
                string val = (mp.GetValue(Model) ?? "").ToString();
                DisplayNameStore store = DisplayNameStore.GetDisplayName(modelType, mp);
                if (!store.IsHidden(isList:true))
                {
                    string display = null;
                    TagBuilder rowTag = new TagBuilder("tr");
                    TagBuilder mpHead = new TagBuilder("th");
                    mpHead.AddCssClass("dl-table-head");
                    mpHead.SetInnerText(store.DisplayName ?? mp.Name);
                    rowTag.InnerHtml += mpHead.ToString();
                    TagBuilder col = new TagBuilder("td");
                    col.AddCssClass("dl-table-data");
                    
                    if(column != null)
                    {
                        if (editable)
                        {
                            display = column.GetEditorFor((HtmlHelper<TModel>)Html, Model);
                        }
                        else
                        {
                            display = column.GetDisplayFor((HtmlHelper<TModel>)Html, Model);
                        }
                    }
                    if (String.IsNullOrWhiteSpace(display))
                    {
                        display += Html.Hidden(mp.Name, val).ToHtmlString();
                        display += val;
                    }
                    col.InnerHtml += display;
                    rowTag.InnerHtml += col.ToString();
                    tableTag.InnerHtml += rowTag.ToString();
                }
                else
                {
                    string display = null;
                    if(column != null)
                    {
                        display = column.GetHiddenFor((HtmlHelper<TModel>)Html, Model);
                    }
                    else
                    {
                        display = Html.Hidden(mp.Name, val).ToHtmlString();
                    }
                    tableWrapper.InnerHtml += display;
                }
            }
            tableWrapper.InnerHtml += tableTag.ToString();
            return MvcHtmlString.Create(tableWrapper.ToString());
        }

        private IHtmlString ToTabularTable(bool sortable, bool editable, string rowHeaderTemplate)
        {
            //Page.SetRequestInfo(Html);
            Type modelType = Page.ModelType;
            PropertyInfo[] modelProps = Page.ModelProperties;
            string pageShortName = Page.Settings.GetShortName();
            string indexPage = Page.Settings.GetIndexPageName();
            string action = MVCVariables.GetRequestActionName();
            string controller = MVCVariables.GetRequestControllerName();
            string currentSort = Page.GetCurrentSort();
            object currentSearch = Page.GetCurrentQuery();
            UrlHelper url = new UrlHelper(Html.ViewContext.RequestContext);

            TagBuilder tableTag = new TagBuilder("table");
            tableTag.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(HtmlAttributes), true);
            tableTag.AddCssClass("table table-builder table-striped table-bordered table-sm table-hover");
            TagBuilder headRowTag = new TagBuilder("tr");
            if (Page.CreateEditControl() || Page.CreateDetailsControl() || Page.CreateDeleteControl() || !String.IsNullOrWhiteSpace(rowHeaderTemplate))
            {
                headRowTag.InnerHtml += new TagBuilder("th").ToString(); //Add Blank column header for row buttons.
            }
            PropertyInfo keyProp = null;
            int colCount = modelProps.Count();
            int usedColCount = 1;
            for (int i = 0; i < colCount; i++)
            {
                PropertyInfo prop = modelProps.ElementAt(i);
                var column = GetColumn(prop.Name);
                keyProp = keyProp ?? (prop.IsKey() ? prop : null);
                DisplayNameStore store = DisplayNameStore.GetDisplayName(modelType, prop);
                if (!store.IsHidden(isTable:true))
                {
                    TagBuilder mpHead = new TagBuilder("th");
                    TagBuilder mpHeadWrap = null;
                    string cn = store?.DisplayName ?? column?.ColumnName ?? prop?.Name;
                    if (Page.CreateSortControl() && sortable)
                    {
                        mpHeadWrap = new TagBuilder("a");
                        mpHeadWrap.AddCssClass("head-wrapper");
                        mpHeadWrap.Attributes.Add("href", url.Action(Page.Settings.GetSortPageName(), controller, new { sortOrder = Page.GetPropertySortPram(prop) }));
                        TagBuilder mpHeadLink = new TagBuilder("span");
                        mpHeadLink.SetInnerText(cn);
                        //mpHead.InnerHtml += Html.ActionLink(store.DisplayName, action, controller, ).ToHtmlString();
                        TagBuilder mpHeadLinkWrap = new TagBuilder("span");
                        mpHeadLinkWrap.AddCssClass("head-link");
                        mpHeadLinkWrap.InnerHtml += mpHeadLink.ToString();
                        mpHeadWrap.InnerHtml += mpHeadLinkWrap.ToString();
                        //mpHead.InnerHtml += Html.SortIdentifier(currentSort, mp.Name);
                        TagBuilder sortIdent = new TagBuilder("i");
                        if (currentSort == Page.GetNameDesc(prop)) sortIdent.AddCssClass("fas fa-sort-down");
                        else if (currentSort == Page.GetNameAsc(prop)) sortIdent.AddCssClass("fas fa-sort-up");
                        else sortIdent.AddCssClass("fas fa-sort");
                        TagBuilder sortIdentWrap = new TagBuilder("span");
                        sortIdentWrap.AddCssClass("head-sort-ident");
                        sortIdentWrap.InnerHtml += sortIdent.ToString();
                        mpHeadWrap.InnerHtml += sortIdentWrap.ToString();
                    }
                    else
                    {
                        mpHeadWrap = new TagBuilder("span");
                        mpHeadWrap.AddCssClass("head-wrapper");
                        mpHeadWrap.SetInnerText(cn);
                    }
                    mpHead.InnerHtml += mpHeadWrap.ToString();
                    headRowTag.InnerHtml += mpHead.ToString();
                    usedColCount++;
                }

            }
            TagBuilder theadTag = new TagBuilder("thead");
            theadTag.InnerHtml += headRowTag.ToString();
            tableTag.InnerHtml += theadTag.ToString();
            TagBuilder tbodyTag = new TagBuilder("tbody");
            int modelCount = ModelArray.Count();
            for (int m = 0; m < modelCount; m++)
            {
                TModel model = ModelArray.ElementAt(m);
                HtmlHelper<TModel> mHtml = new HtmlHelper<TModel>(Html.ViewContext, new ViewDataContainer<TModel>(model));
                TagBuilder rowTag = new TagBuilder("tr");
                if (Page.CreateEditControl() || Page.CreateDetailsControl() || Page.CreateDeleteControl() || !String.IsNullOrWhiteSpace(rowHeaderTemplate))
                {
                    TagBuilder rowButtonsTag = new TagBuilder("td");
                    TagBuilder buttonsWrapper = new TagBuilder("div");
                    buttonsWrapper.AddCssClass("d-flex");
                    if (keyProp != null)
                    {
                        object keyVal = keyProp.GetValue(model);
                        if (Page.CreateEditControl())
                        {
                            //rowButtonsTag.InnerHtml += Html.ActionLink(" ", Page.EditPage, controller,
                            //    new { id = keyVal }, new { @class = "glyphicon glyphicon-pencil", title = "Edit" }).ToHtmlString();
                            TagBuilder editLink = new TagBuilder("a");
                            editLink.MergeAttribute("href", url.Action(Page.Settings.GetEditPageName(), controller, new { id = keyVal }));
                            editLink.AddCssClass("btn btn-info btn-row-icon");
                            TagBuilder editIcon = new TagBuilder("i");
                            editIcon.AddCssClass("fas fa-pencil-alt");
                            editLink.InnerHtml += editIcon.ToString();
                            buttonsWrapper.InnerHtml += editLink.ToString();
                        }
                        //rowButtonsTag.InnerHtml += Html.ActionLink(" ", Page.DetailsPage, controller,
                        //    new { id = keyVal }, new { @class = "glyphicon glyphicon-info-sign", title = "Details" }).ToHtmlString();
                        if (Page.CreateDetailsControl())
                        {
                            TagBuilder detailLink = new TagBuilder("a");
                            detailLink.MergeAttribute("href", Page.GetDetailsPageURL(url, keyVal));
                            detailLink.AddCssClass("btn btn-info btn-row-icon");
                            TagBuilder detailIcon = new TagBuilder("i");
                            detailIcon.AddCssClass("fas fa-info");
                            detailLink.InnerHtml += detailIcon.ToString();
                            buttonsWrapper.InnerHtml += detailLink.ToString();
                        }
                        if (Page.CreateDeleteControl())
                        {
                            //rowButtonsTag.InnerHtml += Html.ActionLink(" ", Page.DeletePage, controller,
                            //        new { id = keyVal }, new { @class = "glyphicon glyphicon-trash", title = "Delete" }).ToHtmlString();
                            TagBuilder deleteLink = new TagBuilder("a");
                            deleteLink.MergeAttribute("href", url.Action(Page.Settings.GetDeletePageName(), controller, new { id = keyVal }));
                            deleteLink.AddCssClass("btn btn-info btn-row-icon");
                            TagBuilder deleteIcon = new TagBuilder("i");
                            deleteIcon.AddCssClass("fas fa-trash-alt");
                            deleteLink.InnerHtml = deleteIcon.ToString();
                            buttonsWrapper.InnerHtml += deleteLink.ToString();
                        }
                    }
                    if (!String.IsNullOrWhiteSpace(rowHeaderTemplate))
                    {
                        buttonsWrapper.InnerHtml += mHtml.Partial(rowHeaderTemplate,
                            new ViewDataDictionary {
                                new KeyValuePair<string, object>(DataKeys.ModelInfo, new ModelInfo(model, null)) });
                    }
                    rowButtonsTag.InnerHtml += buttonsWrapper.ToString();
                    rowTag.InnerHtml += rowButtonsTag.ToString();
                }
                for (int p = 0; p < colCount; p++)
                {
                    PropertyInfo prop = modelProps.ElementAt(p);
                    var column = GetColumn(prop.Name);
                    DisplayNameStore store = DisplayNameStore.GetDisplayName(modelType, prop);
                    if (!store.IsHidden(isTable:true))
                    {
                        TagBuilder col = new TagBuilder("td");
                        col.AddCssClass("table-builder-cell");
                        TagBuilder content = new TagBuilder("span");
                        string val = (prop.GetValue(model) ?? "").ToString();
                        string display = null;
                        if (column != null)
                        {
                            if (editable)
                                display = column.GetEditorFor(mHtml, model);
                            else
                                display = column.GetDisplayFor(mHtml, model);
                        }

                        if (String.IsNullOrWhiteSpace(display))
                        {
                            display = val;
                        }

                        content.InnerHtml += display;
                        //content.SetInnerText();
                        col.InnerHtml += content.ToString();
                        rowTag.InnerHtml += col.ToString();
                    }
                }
                tbodyTag.InnerHtml += rowTag.ToString();
            }
            tableTag.InnerHtml += tbodyTag.ToString();
            if (ModelArray.PageCount > 1 && ShowPager)
            {
                TagBuilder tfootTag = new TagBuilder("tfoot");
                TagBuilder pagerRowTag = new TagBuilder("tr");
                TagBuilder pagerColTag = new TagBuilder("td");
                pagerColTag.Attributes.Add("colspan", $"{usedColCount}");
                TagBuilder pagerTag = new TagBuilder("div");
                pagerTag.AddCssClass("pager");
                string pagerHtml = PagerExtenstions.PagedListPager(Html, ModelArray, page => url.Action(action, controller, new { page })).ToHtmlString();
                pagerHtml = pagerHtml.Replace("<li class=\"active\">", "<li class=\"page-item active\">").Replace("<li>", "<li class\"page-item\">").Replace("<a", "<a class=\"page-link\"");
                pagerTag.InnerHtml = pagerHtml;
                TagBuilder pageNumberTag = new TagBuilder("div");
                pageNumberTag.AddCssClass("text-center");
                pageNumberTag.InnerHtml += $"Page {(ModelArray.PageCount < ModelArray.PageNumber ? 0 : ModelArray.PageNumber)} of {ModelArray.PageCount}";
                pagerTag.InnerHtml += pageNumberTag.ToString();
                pagerColTag.InnerHtml += pagerTag.ToString();
                pagerRowTag.InnerHtml += pagerColTag.ToString();
                tfootTag.InnerHtml += pagerRowTag.ToString();
                tableTag.InnerHtml += tfootTag.ToString();
            }
            TagBuilder divWrapper = new TagBuilder("div");
            divWrapper.AddCssClass("table-wrapper");
            divWrapper.InnerHtml += tableTag.ToString();
            TagBuilder divContainer = new TagBuilder("div");
            divContainer.AddCssClass("container-fluid");
            divContainer.InnerHtml += divWrapper.ToString();
            return MvcHtmlString.Create(divContainer.ToString());
        }
    }
}
