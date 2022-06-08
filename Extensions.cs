using CMWME.C_Extensions;
using MVC.Extensions.Table;
using Newtonsoft.Json;
using X.PagedList;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace MVC.Extensions
{
    public static class Extensions
    {
        
        public static string Id(this HttpContextBase currentContext)
        {
            return currentContext.Request.RequestContext.RouteData.Id();

            //var routeValues = currentContext.Request.RequestContext.RouteData.Values;

            //if (routeValues.ContainsKey("id"))
            //    return (string)routeValues["id"];
            //else if (HttpContext.Current.Request.QueryString.AllKeys.Contains("id"))
            //    return HttpContext.Current.Request.QueryString["id"];

            //return string.Empty;
        }

        public static string GetRequestId(this HttpContext currentContext)
        {
            return currentContext.Request.RequestContext.RouteData.Id();

            //var routeValues = currentContext.Request.RequestContext.RouteData.Values;

            //if (routeValues.ContainsKey("id"))
            //    return (string)routeValues["id"];
            //else if (HttpContext.Current.Request.QueryString.AllKeys.Contains("id"))
            //    return HttpContext.Current.Request.QueryString["id"];

            //return string.Empty;
        }

        public static string Id(this RouteData RouteData)
        {
            var routeValues = RouteData.Values;

            if (routeValues.ContainsKey("id"))
                return (string)routeValues["id"];
            else if (HttpContext.Current.Request.QueryString.AllKeys.Contains("id"))
                return HttpContext.Current.Request.QueryString["id"];

            return string.Empty;
        }



        public static string Controller(this HttpContextBase currentContext)
        {
            return currentContext.Request.RequestContext.RouteData.Controller();

            //var routeValues = currentContext.Request.RequestContext.RouteData.Values;

            //if (routeValues.ContainsKey("controller"))
            //    return (string)routeValues["controller"];

            //return string.Empty;
        }

        public static string GetRequestControllerName(this HttpContext currentContext)
        {
            return currentContext.Request.RequestContext.RouteData.Controller();
        }

        public static string Controller(this RouteData RouteData)
        {
            var routeValues = RouteData.Values;

            if (routeValues.ContainsKey("controller"))
                return (string)routeValues["controller"];

            return string.Empty;
        }


        public static string Action(this HttpContextBase currentContext)
        {
            return currentContext.Request.RequestContext.RouteData.Action();

            //var routeValues = currentContext.Request.RequestContext.RouteData.Values;

            //if (routeValues.ContainsKey("action"))
            //    return (string)routeValues["action"];

            //return string.Empty;
        }

        public static string GetRequestActionName(this HttpContext currentContext)
        {
            return currentContext.Request.RequestContext.RouteData.Action();
        }

        public static string Action(this RouteData RouteData)
        {
            var routeValues = RouteData.Values;

            if (routeValues.ContainsKey("action"))
                return (string)routeValues["action"];

            return string.Empty;
        }


        public static string Area(this HtmlHelper htmlHelper)
        {
            var dataTokens = HttpContext.Current.Request.RequestContext.RouteData.DataTokens;

            if (dataTokens.ContainsKey("area"))
                return (string)dataTokens["area"];

            return string.Empty;
        }

        /// <summary>
        /// Used to determine the direction of the sort identifier used when filtering lists
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="sortOrder">the current sort order being used on the page</param>
        /// <param name="field">the field that we are attaching this sort identifier to</param>
        /// <returns>MvcHtmlString used to indicate the sort order of the field</returns>
        public static IHtmlString SortIdentifier(this HtmlHelper htmlHelper, string sortOrder, string field)
        {
            if (string.IsNullOrEmpty(sortOrder) || (sortOrder.Trim() != field && sortOrder.Replace("_desc", "").Trim() != field)) return null;

            string glyph = "glyphicon glyphicon-chevron-up";
            if (sortOrder.ToLower().Contains("desc"))
            {
                glyph = "glyphicon glyphicon-chevron-down";
            }

            var span = new TagBuilder("span");
            span.Attributes["class"] = glyph;

            return MvcHtmlString.Create(span.ToString());
        }

        /// <summary>
        /// Converts a NameValueCollection into a RouteValueDictionary containing all of the elements in the collection, and optionally appends
        /// {newKey: newValue} if they are not null
        /// </summary>
        /// <param name="collection">NameValue collection to convert into a RouteValueDictionary</param>
        /// <param name="newKey">the name of a key to add to the RouteValueDictionary</param>
        /// <param name="newValue">the value associated with newKey to add to the RouteValueDictionary</param>
        /// <returns>A RouteValueDictionary containing all of the keys in collection, as well as {newKey: newValue} if they are not null</returns>
        public static RouteValueDictionary ToRouteValueDictionary(this NameValueCollection collection, string newKey, string newValue)
        {
            var routeValueDictionary = new RouteValueDictionary();
            foreach (var key in collection.AllKeys)
            {
                if (key == null) continue;
                if (routeValueDictionary.ContainsKey(key))
                    routeValueDictionary.Remove(key);

                routeValueDictionary.Add(key, collection[key]);
            }
            if (string.IsNullOrEmpty(newValue))
            {
                routeValueDictionary.Remove(newKey);
            }
            else
            {
                if (routeValueDictionary.ContainsKey(newKey))
                    routeValueDictionary.Remove(newKey);

                routeValueDictionary.Add(newKey, newValue);
            }
            return routeValueDictionary;
        }



        public static string ToJson(this object obj) 
        { 
             JsonSerializer js = JsonSerializer.Create(new JsonSerializerSettings()); 
             var jw = new StringWriter(); 
             js.Serialize(jw, obj);
             string jsStr = jw.ToString();
             //if (!String.IsNullOrWhiteSpace(jsStr)) { jsStr = jsStr.Replace("<", "&lt;").Replace(">", "&gt;"); }
             return jsStr; 
         }

        public static void ClearErrors(this ModelStateDictionary modelstate)
        {
            for (int i = modelstate.Keys.Count - 1; i >= 0; i--)
            {
                List<string> keys = new List<string>() {  };
                keys.AddRange(modelstate.Keys);
                modelstate[keys[i]].Errors.Clear();
            }
        }

        public static IHtmlString ConvertHtmlAttributes(this HtmlHelper html, object htmlAttributes)
        {
            RouteValueDictionary attriDict = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            StringBuilder attriStr = new StringBuilder(); string c = "";
            for (int i = 0; i < attriDict.Count; i++)
            {
                KeyValuePair<string, object> pair = attriDict.ElementAt(i);
                attriStr.Append($"{c}{pair.Key}=\"{pair.Value}\"");
                c = " ";
            }
            return MvcHtmlString.Create(attriStr.ToString());
        }

        public static TableBuilder<TModel> GetResponsiveTable<TModel>(this HtmlHelper<IPagedList<TModel>> html, 
            IPagedList<TModel> modelArray, IPageSchema page, object htmlAttributes = null, bool showPager = false) where TModel : class
        {
            return new TableBuilder<TModel>(html, modelArray,page, htmlAttributes, showPager);
            //page.SetRequestInfo(html);
            //Type modelType = page.ModelType;
            //PropertyInfo[] modelProps = page.ModelProperties;
            //string PageShortName = page.ShortName;
            //string PageName = page.Name;
            //string action = page.ActionName;
            //string controller = page.ControllerName;
            //var user = HttpContext.Current.User;
            //string currentSort = page.GetCurrentSort();
            //object currentSearch = page.GetCurrentQuery();
            //UrlHelper url = new UrlHelper(html.ViewContext.RequestContext);

            //TagBuilder tableTag = new TagBuilder("table");
            //tableTag.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), true);
            //tableTag.AddCssClass("table table-striped table-sm table-hover");
            //TagBuilder headRowTag = new TagBuilder("tr");
            //headRowTag.InnerHtml += new TagBuilder("th").ToString(); //Add Blank column header for row buttons.
            //PropertyInfo keyProp = null;
            //int colCount = modelProps.Count();
            //int usedColCount = 1;
            //for (int i = 0; i < colCount; i++)
            //{
            //    PropertyInfo mp = modelProps.ElementAt(i);
            //    keyProp = keyProp ?? (mp.IsKey() ? mp : null);
            //    DisplayNameStore store = DisplayNameStore.GetDisplayName(modelType, mp);
            //    if (store.DisplayName != null)
            //    {
                    
            //        TagBuilder mpHeadWrap = new TagBuilder("a");
            //        mpHeadWrap.AddCssClass("head-wrapper");
            //        mpHeadWrap.Attributes.Add("href", url.Action(action, controller, new { sortOrder = page.GetPropertySortPram(mp) }));
            //        TagBuilder mpHeadLink = new TagBuilder("span");
            //        mpHeadLink.SetInnerText(store.DisplayName);
            //        //mpHead.InnerHtml += html.ActionLink(store.DisplayName, action, controller, ).ToHtmlString();
            //        TagBuilder mpHeadLinkWrap = new TagBuilder("span");
            //        mpHeadLinkWrap.AddCssClass("head-link");
            //        mpHeadLinkWrap.InnerHtml += mpHeadLink.ToString();
            //        mpHeadWrap.InnerHtml += mpHeadLinkWrap.ToString();
            //        //mpHead.InnerHtml += html.SortIdentifier(currentSort, mp.Name);
            //        TagBuilder sortIdent = new TagBuilder("i");
            //        if (currentSort == mp.GetNameDesc()) sortIdent.AddCssClass("fas fa-sort-down");
            //        else if (currentSort == mp.GetNameAsc()) sortIdent.AddCssClass("fas fa-sort-up");
            //        else sortIdent.AddCssClass("fas fa-sort");
            //        TagBuilder sortIdentWrap = new TagBuilder("span");
            //        sortIdentWrap.AddCssClass("head-sort-ident");
            //        sortIdentWrap.InnerHtml += sortIdent.ToString();
            //        mpHeadWrap.InnerHtml += sortIdentWrap.ToString();
            //        TagBuilder mpHead = new TagBuilder("th");
            //        mpHead.InnerHtml += mpHeadWrap.ToString();
            //        headRowTag.InnerHtml += mpHead.ToString();
            //        usedColCount++;
            //    }

            //}
            //TagBuilder theadTag = new TagBuilder("thead");
            //theadTag.InnerHtml += headRowTag.ToString();
            //tableTag.InnerHtml += theadTag.ToString();
            //TagBuilder tbodyTag = new TagBuilder("tbody");
            //int modelCount = modelArray.Count();
            //for (int m = 0; m < modelCount; m++)
            //{
            //    T model = modelArray.ElementAt(m);
            //    TagBuilder rowTag = new TagBuilder("tr");
            //    TagBuilder rowButtonsTag = new TagBuilder("td");
            //    if (keyProp != null)
            //    {
            //        object keyVal = keyProp.GetValue(model);
            //        if (page.CreateEditControl())
            //        {
            //            //rowButtonsTag.InnerHtml += html.ActionLink(" ", page.EditPage, controller,
            //            //    new { id = keyVal }, new { @class = "glyphicon glyphicon-pencil", title = "Edit" }).ToHtmlString();
            //            TagBuilder editLink = new TagBuilder("a");
            //            editLink.Attributes.Add("href", url.Action(page.EditPage, controller, new { id = keyVal }));
            //            editLink.AddCssClass("btn btn-info btn-row-icon");
            //            TagBuilder editIcon = new TagBuilder("i");
            //            editIcon.AddCssClass("fas fa-pencil-alt");
            //            editLink.InnerHtml += editIcon.ToString();
            //            rowButtonsTag.InnerHtml += editLink.ToString();
            //        }
            //        //rowButtonsTag.InnerHtml += html.ActionLink(" ", page.DetailsPage, controller,
            //        //    new { id = keyVal }, new { @class = "glyphicon glyphicon-info-sign", title = "Details" }).ToHtmlString();
            //        TagBuilder detailLink = new TagBuilder("a");
            //        detailLink.Attributes.Add("href", url.Action(page.DetailsPage, controller, new { id = keyVal }));
            //        detailLink.AddCssClass("btn btn-info btn-row-icon");
            //        TagBuilder detailIcon = new TagBuilder("i");
            //        detailIcon.AddCssClass("fas fa-info");
            //        detailLink.InnerHtml += detailIcon.ToString();
            //        rowButtonsTag.InnerHtml += detailLink.ToString();
            //        if (page.CreateDeleteControl())
            //        {
            //            //rowButtonsTag.InnerHtml += html.ActionLink(" ", page.DeletePage, controller,
            //            //        new { id = keyVal }, new { @class = "glyphicon glyphicon-trash", title = "Delete" }).ToHtmlString();
            //            TagBuilder deleteLink = new TagBuilder("a");
            //            deleteLink.Attributes.Add("href", url.Action(page.DeletePage, controller, new { id = keyVal }));
            //            deleteLink.AddCssClass("btn btn-info btn-row-icon");
            //            TagBuilder deleteIcon = new TagBuilder("i");
            //            deleteIcon.AddCssClass("fas fa-trash-alt");
            //            deleteLink.InnerHtml = deleteIcon.ToString();
            //            rowButtonsTag.InnerHtml += deleteLink.ToString();
            //        }
            //    }
            //    rowTag.InnerHtml += rowButtonsTag.ToString();
                
            //    for (int p = 0; p < colCount; p++)
            //    {
            //        PropertyInfo prop = modelProps.ElementAt(p);
            //        DisplayNameStore store = DisplayNameStore.GetDisplayName(modelType, prop);
            //        if (store.DisplayName != null)
            //        {
            //            TagBuilder col = new TagBuilder("td");
            //            TagBuilder content = new TagBuilder("span");
            //            string val = (prop.GetValue(model) ?? "").ToString();
            //            string display = "";// html.Display(val, prop.Name, prop.Name).ToString();
            //            //if (String.IsNullOrWhiteSpace(display))
            //            //{
            //                display = val;
            //            //}
                        
            //            col.SetInnerText(display);
            //            //content.SetInnerText();
            //            col.InnerHtml += content.ToString();
            //            rowTag.InnerHtml += col.ToString();
            //        }
            //    }
            //    tbodyTag.InnerHtml += rowTag.ToString();
            //}
            //tableTag.InnerHtml += tbodyTag.ToString();
            //if(modelArray.PageCount > 1)
            //{
            //    TagBuilder tfootTag = new TagBuilder("tfoot");
            //    TagBuilder pagerRowTag = new TagBuilder("tr");
            //    TagBuilder pagerColTag = new TagBuilder("td");
            //    pagerColTag.Attributes.Add("colspan", $"{usedColCount}");
            //    TagBuilder pagerTag = new TagBuilder("div");
            //    pagerTag.AddCssClass("pager");
            //    string pagerHtml = PagerExtenstions.PagedListPager(html, modelArray, pager => url.Action(action, controller, new { pager, sortOrder = currentSort, query = currentSearch })).ToHtmlString();
            //    pagerHtml = pagerHtml.Replace("<li class=\"active\">", "<li class=\"page-item active\">").Replace("<li>", "<li class\"page-item\">").Replace("<a", "<a class=\"page-link\"");
            //    pagerTag.InnerHtml = pagerHtml;
            //    TagBuilder pageNumberTag = new TagBuilder("div");
            //    pageNumberTag.AddCssClass("text-center");
            //    pageNumberTag.InnerHtml += $"Page {(modelArray.PageCount < modelArray.PageNumber ? 0 : modelArray.PageNumber)} of {modelArray.PageCount}";
            //    pagerTag.InnerHtml += pageNumberTag.ToString();
            //    pagerColTag.InnerHtml += pagerTag.ToString();
            //    pagerRowTag.InnerHtml += pagerColTag.ToString();
            //    tfootTag.InnerHtml += pagerRowTag.ToString();
            //    tableTag.InnerHtml += tfootTag.ToString();
            //}
            //TagBuilder divWrapper = new TagBuilder("div");
            //divWrapper.AddCssClass("table-responsive");
            //divWrapper.InnerHtml += tableTag.ToString();
            //TagBuilder divContainer = new TagBuilder("div");
            //divContainer.AddCssClass("container-fluid");
            //divContainer.InnerHtml += divWrapper.ToString();
            //return MvcHtmlString.Create(divContainer.ToString());
        }

        public static IHtmlString GetBaseQueryForm(this HtmlHelper html, IPageSchema pageInfo, bool textArea = false, bool fullFilter = false) 
        {
            UrlHelper url = pageInfo.GetCurrentUrl();

            TagBuilder formTag = new TagBuilder("form");
            formTag.MergeAttribute("method", "post");
            formTag.MergeAttribute("action", pageInfo.GetFilterPageURL(url));
            TagBuilder formRow = new TagBuilder("div");
            formRow.AddCssClass("form-row align-items-top");
            //formColSearch.AddCssClass("col-auto");
            object textVal = pageInfo.GetCurrentQuery();
            IEnumerable<IPageFilter> filters = null;
            if (!textVal.IsType<string>())
            {
                if (!textVal.IsType<IEnumerable<IPageFilter>>(out filters))
                {
                    if(textVal.IsType(out IEnumerable<PageFilter> pgfilters))
                    {
                        filters = pgfilters;
                    }
                }
            }
            
            if (filters?.GetCount() > 0 || fullFilter)
            {
                IPageFilter f = new PageFilter();
                if (filters.GetCount() > 0)
                {
                    f = filters.ElementAt(0);
                }
                formRow.InnerHtml += pageInfo.GetFilterControl(html, f, 0, textArea);
            }
            else
            {
                TagBuilder formColSearch = new TagBuilder("span");
                TagBuilder searchBox = null;
                if (!textArea)
                {
                    searchBox = new TagBuilder("input");
                    searchBox.Attributes.Add("value", textVal as string);
                }
                else
                {
                    searchBox = new TagBuilder("textarea");
                    searchBox.MergeAttribute("cols", "20");
                    searchBox.MergeAttribute("rows", "5");
                    searchBox.SetInnerText(textVal as string);
                }
                searchBox.AddCssClass("search-text form-control");
                searchBox.Attributes.Add("id", "txtbx_search");
                searchBox.Attributes.Add("name", "query");
                searchBox.Attributes.Add("type", "text");

                searchBox.Attributes.Add("placeholder", "Search...");
                formColSearch.InnerHtml += searchBox.ToString();
                formRow.InnerHtml += formColSearch.ToString();
            }

            TagBuilder formColSearchBtn = new TagBuilder("div");
            formColSearchBtn.AddCssClass("col-auto");
            TagBuilder searchBtn = new TagBuilder("button");
            searchBtn.AddCssClass("btn btn-info btn-icon");
            searchBtn.Attributes.Add("id","btn_search");
            searchBtn.Attributes.Add("title", "Search");
            TagBuilder searchBtnIcon = new TagBuilder("i");
            searchBtnIcon.AddCssClass("fas fa-search");
            searchBtnIcon.Attributes.Add("id", "btn_create");
            searchBtn.InnerHtml += searchBtnIcon.ToString();
            formColSearchBtn.InnerHtml += searchBtn.ToString();
            formRow.InnerHtml += formColSearchBtn.ToString();
            //if (pageInfo.CreateClearControl())
            //{
            if (pageInfo.CreateClearControl())
            {
                TagBuilder formColClear = new TagBuilder("div");
                formColClear.AddCssClass("col-auto");
                TagBuilder clearControl = new TagBuilder("a");
                clearControl.AddCssClass("btn btn-info btn-icon");
                clearControl.Attributes.Add("href", pageInfo.GetClearPageURL(url));
                clearControl.Attributes.Add("id", "btn_clear");
                clearControl.Attributes.Add("title", "Clear Search");
                TagBuilder clearControlIcon = new TagBuilder("i");
                clearControlIcon.AddCssClass("fas fa-times");
                clearControl.InnerHtml += clearControlIcon.ToString();
                formColClear.InnerHtml += clearControl.ToString();
                formRow.InnerHtml += formColClear.ToString();
            }
            //}
            if (pageInfo.CreateCreateControl())
            {
                TagBuilder formColCreate = new TagBuilder("div");
                formColCreate.AddCssClass("col-auto");
                TagBuilder createControl = new TagBuilder("a");
                createControl.AddCssClass("btn btn-info btn-icon");
                createControl.Attributes.Add("href", url.Action(pageInfo.Settings.GetCreatePageName(), MVCVariables.GetRequestControllerName()));
                createControl.Attributes.Add("id", "btn_create");
                createControl.Attributes.Add("title", "Create New");
                TagBuilder createIcon = new TagBuilder("i");
                createIcon.AddCssClass("fas fa-plus");
                createControl.InnerHtml += createIcon.ToString();
                formColCreate.InnerHtml += createControl.ToString();
                formRow.InnerHtml += formColCreate.ToString();
            }
            formTag.InnerHtml += formRow.ToString();
            TagBuilder boxTag = new TagBuilder("div");
            boxTag.AddCssClass("search-box inline-flex");
            boxTag.InnerHtml += formTag.ToString();
            return MvcHtmlString.Create(boxTag.ToString());
        }

        public static IHtmlString GetFilterControl(this HtmlHelper html, IPageSchema pageInfo, IPageFilter filter = null, int index = 0, bool textArea = false)
        {
            TagBuilder wrapper = new TagBuilder("div");
            wrapper.AddCssClass("form-row");
            //Build the Field Control
            TagBuilder fieldWrapper = new TagBuilder("div");
            fieldWrapper.AddCssClass("col-auto");
            TagBuilder fieldCon = new TagBuilder("select");
            fieldCon.AddCssClass("form-control");
            string fieldName = $"filters[{index}].Field";
            fieldCon.MergeAttribute("id", fieldName);
            fieldCon.MergeAttribute("name", fieldName);
            TagBuilder allOption = new TagBuilder("option");
            allOption.MergeAttribute("value", DataKeys.AllOption);
            allOption.SetInnerText("Search All");
            fieldCon.InnerHtml += allOption.ToString();
            Type t = pageInfo.ModelType;
            PropertyInfo[] props = t.GetProperties();
            for (int i = 0; i < props.Length; i++)
            {
                PropertyInfo p = props[i];
                string displayName = t.GetDisplayName(p, true);
                if (displayName != null)
                {
                    TagBuilder optionTag = new TagBuilder("option");
                    if (filter != null && filter.Field == p.Name) optionTag.MergeAttribute("selected", "selected");
                    optionTag.MergeAttribute("value", p.Name);
                    optionTag.SetInnerText(displayName);
                    fieldCon.InnerHtml += optionTag.ToString();
                }
            }
            fieldWrapper.InnerHtml += fieldCon.ToString();
            wrapper.InnerHtml += fieldWrapper.ToString();

            //Build Condition Control
            TagBuilder conWrapper = new TagBuilder("div");
            conWrapper.AddCssClass("col-auto");
            TagBuilder conCon = new TagBuilder("select");
            conCon.AddCssClass("form-control");
            string conName = $"filters[{index}].Condition";
            conCon.MergeAttribute("id", conName);
            conCon.MergeAttribute("name", conName);
            var conditions = typeof(PageFilter.FilterCondition).GetEnumValues();
            foreach (PageFilter.FilterCondition item in conditions)
            {
                string conDisplayName = item.GetEnumDisplayName(true);
                if (conDisplayName != null)
                {
                    string conValue = item.ToString();
                    TagBuilder optionTag = new TagBuilder("option");
                    if (filter != null && filter.Condition == conValue)
                        optionTag.MergeAttribute("selected", "selected");
                    optionTag.MergeAttribute("value", conValue);
                    optionTag.SetInnerText(conDisplayName);
                    conCon.InnerHtml += optionTag.ToString();
                }
            }
            conWrapper.InnerHtml += conCon.ToString();
            wrapper.InnerHtml += conWrapper.ToString();

            //Build Value Control
            TagBuilder textWrapper = new TagBuilder("div");
            textWrapper.AddCssClass("col-auto");
            string textConName = $"filters[{index}].Value";
            TagBuilder textCon = null;
            if (textArea)
            {
                textCon = new TagBuilder("textarea");
                textCon.MergeAttribute("cols", "20");
                textCon.MergeAttribute("rows", "5");
                textCon.SetInnerText(filter.Value);
            }
            else
            {
                textCon = new TagBuilder("Input");
                textCon.MergeAttribute("type", "text");
                if(filter != null)
                {
                    textCon.MergeAttribute("value", filter.Value);
                }
            }
            textCon.AddCssClass("form-control");
            textCon.MergeAttribute("id", textConName);
            textCon.MergeAttribute("name", textConName);
            textWrapper.InnerHtml += textCon.ToString();
            wrapper.InnerHtml += textWrapper.ToString();

            return MvcHtmlString.Create(wrapper.ToString());
        }

        public static IHtmlString GetFormControls(this HtmlHelper html, IPageSchema pageInfo, bool enableEdit = false, int? keyId = null)
        {
            UrlHelper url = pageInfo.GetCurrentUrl();

            TagBuilder wrapper = new TagBuilder("span");
            wrapper.GenerateId("btn_back");
            wrapper.AddCssClass("inline-flex");
            TagBuilder backWrapper = new TagBuilder("span");
            backWrapper.AddCssClass("col-auto");
            TagBuilder backCon = new TagBuilder("button");
            backCon.AddCssClass("btn btn-info btn-icon");
            backCon.MergeAttribute("title","Back");
            //string hrefUrl = HttpContext.Current.Request.UrlReferrer?.PathAndQuery 
            //    ?? url.Action(pageInfo.PageName, pageInfo.ControllerName);
            //if (hrefUrl.IndexOf(pageInfo.DetailsPage ?? "") >= 0 || hrefUrl.IndexOf(pageInfo.EditPage ?? "") >= 0 ||
            //    hrefUrl.IndexOf(pageInfo.DeletePage ?? "") >= 0 || hrefUrl.IndexOf(pageInfo.CreatePage ?? "") >= 0)
            //{
            //    hrefUrl = url.Action(pageInfo.PageName, pageInfo.ControllerName);
            //}

            //backCon.MergeAttribute("href", hrefUrl);
            backCon.MergeAttribute("onclick", "window.history.back()");
            TagBuilder backIcon = new TagBuilder("i");
            backIcon.AddCssClass("fas fa-arrow-left");
            backCon.InnerHtml += backIcon.ToString();
            backWrapper.InnerHtml += backCon.ToString();
            wrapper.InnerHtml += backWrapper.ToString();
            if (enableEdit && keyId != null)
            {
                TagBuilder editWrapper = new TagBuilder("span");
                editWrapper.AddCssClass("col-auto");
                TagBuilder editControl = new TagBuilder("a");
                editControl.AddCssClass("btn btn-info btn-icon");
                editControl.MergeAttribute("href", url.Action(pageInfo.Settings.GetEditPageName(), MVCVariables.GetRequestControllerName(), new { id = keyId }));
                TagBuilder editIcon = new TagBuilder("i");
                editIcon.AddCssClass("fas fa-pencil-alt");
                editControl.InnerHtml += editIcon.ToString();
                editWrapper.InnerHtml += editControl.ToString();
                wrapper.InnerHtml += editWrapper.ToString();
            }
            return MvcHtmlString.Create(wrapper.ToString());
        }

        public static TableBuilder<TModel> GetResponsiveList<TModel>(this HtmlHelper<TModel> html,
            TModel model, IPageSchema page, object htmlAttributes = null) where TModel : class
        {
            return new TableBuilder<TModel>(html, model, page, htmlAttributes);
        }

        public static IHtmlString GetRecordCount(this HtmlHelper html, IPagedList model)
        {
            TagBuilder recordCountWrap = new TagBuilder("span");
            recordCountWrap.AddCssClass("record-count col-auto");
            recordCountWrap.SetInnerText($"{model.TotalItemCount} results");
            return MvcHtmlString.Create(recordCountWrap.ToString());
        }

        public static IHtmlString GetPager(this HtmlHelper html, IPagedList model, IPageSchema pageInfo)
        {
            UrlHelper url = pageInfo.GetCurrentUrl();

            TagBuilder PagerWrap = new TagBuilder("div");
            PagerWrap.AddCssClass("pager-header");
            TagBuilder Pager = new TagBuilder("form");
            Pager.AddCssClass("pager-form");
            Pager.MergeAttribute("method", "get");
            Pager.MergeAttribute("action", url.Action(MVCVariables.GetRequestActionName(), MVCVariables.GetRequestControllerName()));
            //Pager.InnerHtml += PagerExtenstions.PagedListPagerLimit5(html, model, pager => url.Action(pageInfo.ActionName, pageInfo.ControllerName, new { pager }));
            TagBuilder pagerFirst = null;
            if (!model.IsFirstPage)
            {
                pagerFirst = new TagBuilder("a");
                pagerFirst.MergeAttribute("href", url.Action(MVCVariables.GetRequestActionName(), MVCVariables.GetRequestControllerName(), new { page = 1 }));
            }
            else
            {
                pagerFirst = new TagBuilder("span");
            }
            pagerFirst.AddCssClass("pager-left-icon");
            TagBuilder pagerFirstIcon = new TagBuilder("i");
            pagerFirstIcon.AddCssClass("fas fa-angle-double-left");
            pagerFirst.InnerHtml += pagerFirstIcon.ToString();
            Pager.InnerHtml += pagerFirst;
            TagBuilder pagerPrevious = null;
            if (!model.IsFirstPage)
            {
                pagerPrevious = new TagBuilder("a");
                pagerPrevious.MergeAttribute("href", url.Action(MVCVariables.GetRequestActionName(), MVCVariables.GetRequestControllerName(), new { page = model.PageNumber -1 }));
            }
            else
            {
                pagerPrevious = new TagBuilder("span");
            }
            pagerPrevious.AddCssClass("");
            TagBuilder pagerPreviousIcon = new TagBuilder("i");
            pagerPreviousIcon.AddCssClass("fas fa-angle-left");
            pagerPrevious.InnerHtml += pagerPreviousIcon.ToString();
            Pager.InnerHtml += pagerPrevious;

            TagBuilder rightPagerText = new TagBuilder("span");
            rightPagerText.AddCssClass("col-auto");
            rightPagerText.SetInnerText("Page ");
            Pager.InnerHtml += rightPagerText.ToString();

            IEnumerable<int> pageList = PagerExtenstions.GetPageList(model);
            //string disabled = "";
            if (model.PageCount <= 1) 
                Pager.InnerHtml += html.DropDownList("page", new SelectList(pageList, model.PageNumber), new { @class = $"form-control inline-flex", disabled="disabled" }).ToHtmlString();
            else
                Pager.InnerHtml += html.DropDownList("page", new SelectList(pageList, model.PageNumber), new { @class = $"form-control inline-flex", submit_form="" }).ToHtmlString();
            TagBuilder leftPagerText = new TagBuilder("span");
            leftPagerText.AddCssClass("col-auto");
            leftPagerText.SetInnerText($" of {model.PageCount}");
            Pager.InnerHtml += leftPagerText.ToString();
            TagBuilder pagerNext = null;
            if (!model.IsLastPage)
            {
                pagerNext = new TagBuilder("a");
                pagerNext.MergeAttribute("href", url.Action(MVCVariables.GetRequestActionName(), MVCVariables.GetRequestControllerName(), new { page = model.PageNumber + 1 }));
            }
            else
            {
                pagerNext = new TagBuilder("span");
            }
            pagerNext.AddCssClass("");
            TagBuilder pagerNextIcon = new TagBuilder("i");
            pagerNextIcon.AddCssClass("fas fa-angle-right");
            pagerNext.InnerHtml += pagerNextIcon.ToString();
            Pager.InnerHtml += pagerNext;
            TagBuilder pagerLast = null;
            if (!model.IsLastPage)
            {
                pagerLast = new TagBuilder("a");
                pagerLast.MergeAttribute("href", url.Action(MVCVariables.GetRequestActionName(), MVCVariables.GetRequestControllerName(), new { page = model.PageCount }));
            }
            else
            {
                pagerLast = new TagBuilder("span");
            }
            pagerLast.AddCssClass("pager-right-icon");
            TagBuilder pagerLastIcon = new TagBuilder("i");
            pagerLastIcon.AddCssClass("fas fa-angle-double-right");
            pagerLast.InnerHtml += pagerLastIcon.ToString();
            Pager.InnerHtml += pagerLast;
            PagerWrap.InnerHtml += Pager.ToString();
            //TagBuilder pageNumberTag = new TagBuilder("span");
            //pageNumberTag.AddCssClass("text-center");
            //pageNumberTag.InnerHtml += $"Page {(model.PageCount < model.PageNumber ? 0 : model.PageNumber)} of {model.PageCount}";
            //PagerWrap.InnerHtml += pageNumberTag.ToString();

            PagerWrap.InnerHtml += html.GetRecordCount(model).ToHtmlString();
            return MvcHtmlString.Create(PagerWrap.ToString());
        }

        public static IHtmlString GetTableMenu(this HtmlHelper html, IPageSchema pageInfo,
            bool ShowExport = false, bool ShowSave = false, bool ShowLoad = false, string menuTemplate = null)
        {
            UrlHelper url = pageInfo.GetCurrentUrl();
            
            TagBuilder menuContainer = new TagBuilder("div");
            menuContainer.AddCssClass("nav-menu");
            //TagBuilder navbar = new TagBuilder("nav");
            //navbar.AddCssClass("navbar");
            //TagBuilder placeholder = new TagBuilder("ul");
            //placeholder.AddCssClass("nav navbar-nav navbar-right");
            //TagBuilder placeholderInner = new TagBuilder("li");
            //placeholderInner.AddCssClass("dropdown");
            //placeholder.InnerHtml += placeholderInner.ToString();
            //navbar.InnerHtml += placeholder.ToString();
            TagBuilder container = new TagBuilder("div");
            container.AddCssClass("dropdown show");
            //TagBuilder header = new TagBuilder("div");
            //header.AddCssClass("navbar-header");
            TagBuilder headerButton = new TagBuilder("a");
            headerButton.GenerateId("DropdownMenuLink");
            headerButton.AddCssClass("btn btn-info btn-icon dropdown-toggle");
            headerButton.MergeAttribute("href", "http://example.com");
            headerButton.MergeAttribute("data-toggle", "dropdown");
            headerButton.MergeAttribute("aria-expanded", "false");
            headerButton.MergeAttribute("aria-haspopup", "true");
            //headerButton.MergeAttribute("data-target", "#navbarResponsive");
            //headerButton.MergeAttribute("aria-controls", "navbarResponsive");
            //headerButton.MergeAttribute("aria-expanded", "false");
            //headerButton.MergeAttribute("aria-label", "Toggle navigation");
            TagBuilder headerIcon = new TagBuilder("i");
            headerIcon.AddCssClass("fas fa-cog");
            headerButton.InnerHtml += headerIcon.ToString();
            //headerButton.InnerHtml += headerIcon.ToString();
            //headerButton.InnerHtml += headerIcon.ToString();
            //header.InnerHtml += headerButton.ToString();
            //container.InnerHtml += header.ToString();
            container.InnerHtml += headerButton.ToString();
            //TagBuilder navBarMenuContainer = new TagBuilder("div");
            //navBarMenuContainer.GenerateId("dropdownMenuLink");
            //navBarMenuContainer.AddCssClass("collapse nav-collapse");
            TagBuilder navBarMenu = new TagBuilder("ul");
            navBarMenu.AddCssClass("dropdown-menu");
            navBarMenu.MergeAttribute("aria-labelledby", "DropdownMenuLink");
            //navBarMenu.MergeAttribute("role", "menu");
            //navBarMenu.MergeAttribute("data-dropdown-menu", "");

            if (pageInfo.CreatePageSizeControl())
            {
                TagBuilder pageSizer = new TagBuilder("li");
                pageSizer.AddCssClass("dropdown-submenu");
                TagBuilder pageSizerToggle = new TagBuilder("a");
                pageSizerToggle.AddCssClass("dropdown-item dropdown-toggle");
                pageSizerToggle.MergeAttribute("href", "#");
                pageSizerToggle.MergeAttribute("tabindex", "-1");
                //pageSizerToggle.MergeAttribute("data-toggle", "dropdown");
                pageSizerToggle.SetInnerText("Page Size");
                //TagBuilder rightIcon = new TagBuilder("i");
                //rightIcon.AddCssClass("icon-chevron-right");
                //pageSizerToggle.InnerHtml += rightIcon.ToString();
                pageSizer.InnerHtml += pageSizerToggle.ToString();
                TagBuilder pageSizerSubContainer = new TagBuilder("ul");
                pageSizerSubContainer.AddCssClass("dropdown-menu");
                foreach(var pSize in pageInfo.Settings.GetPageSizeList())
                { 
                    TagBuilder pageSizerChild = new TagBuilder("li");
                    if (pageInfo.GetCurrentPageSize() == pSize)
                    {
                        pageSizerChild.AddCssClass("dropdown-item active");
                        pageSizerChild.SetInnerText($"{pSize}");
                    }
                    else
                    {
                        TagBuilder pageSizerAction = new TagBuilder("a");
                        pageSizerAction.AddCssClass("dropdown-item");
                        pageSizerAction.MergeAttribute("href", url.Action(pageInfo.Settings.GetSizePageName(), MVCVariables.GetRequestControllerName(), new { pageSize = pSize }));
                        pageSizerAction.SetInnerText($"{pSize}");
                        pageSizerChild.InnerHtml += pageSizerAction.ToString();
                    }
                    pageSizerSubContainer.InnerHtml += pageSizerChild.ToString();
                }
                pageSizer.InnerHtml += pageSizerSubContainer.ToString();
                navBarMenu.InnerHtml += pageSizer.ToString();
            }
            if(ShowExport && pageInfo.CreateExportControl())
            {
                TagBuilder ExportControl = new TagBuilder("li");
                ExportControl.AddCssClass("");
                TagBuilder ExportControlLink = new TagBuilder("a");
                ExportControlLink.AddCssClass("dropdown-item");
                ExportControlLink.MergeAttribute("href", url.Action(pageInfo.Settings.GetExportPageName(), MVCVariables.GetRequestControllerName()));
                ExportControlLink.SetInnerText("Export CSV");
                ExportControl.InnerHtml += ExportControlLink.ToString();
                navBarMenu.InnerHtml += ExportControl.ToString();
            }
            if(ShowLoad && pageInfo.CreateLoadControl())
            {
                TagBuilder loadControl = new TagBuilder("li"); 
                loadControl.AddCssClass("dropdown-submenu");
                TagBuilder loadControlToggle = new TagBuilder("a");
                loadControlToggle.AddCssClass("dropdown-item dropdown-toggle");
                loadControlToggle.MergeAttribute("href", "#");
                loadControlToggle.MergeAttribute("tabindex", "-1");
                //pageSizerToggle.MergeAttribute("data-toggle", "dropdown");
                loadControlToggle.SetInnerText("Load Filter");
                //TagBuilder rightIcon = new TagBuilder("i");
                //rightIcon.AddCssClass("icon-chevron-right");
                //pageSizerToggle.InnerHtml += rightIcon.ToString();
                loadControl.InnerHtml += loadControlToggle.ToString();
                TagBuilder loadControlSubContainer = new TagBuilder("ul");
                loadControlSubContainer.AddCssClass("dropdown-menu");
                for (int i = 0; i < pageInfo.Reports.Count(); i++)
                {
                    string report = pageInfo.Reports.ElementAt(i);
                    TagBuilder loadControlReportContainer = new TagBuilder("li");
                    TagBuilder reportLink = new TagBuilder("a");
                    reportLink.AddCssClass("dropdown-item");
                    reportLink.MergeAttribute("href", url.Action(pageInfo.Settings.GetLoadFilterPageName(), MVCVariables.GetRequestControllerName(), new { loadFilter = report }));
                    reportLink.SetInnerText($"{report}");
                    loadControlReportContainer.InnerHtml += reportLink.ToString();
                    loadControlSubContainer.InnerHtml += loadControlReportContainer.ToString();
                }
                loadControl.InnerHtml += loadControlSubContainer.ToString();
                navBarMenu.InnerHtml += loadControl.ToString();
            }
            if(ShowSave && pageInfo.CreateSaveControl())
            {
                TagBuilder saveControl = new TagBuilder("li");
                saveControl.AddCssClass("dropdown-submenu");
                TagBuilder SaveControlToggle = new TagBuilder("a");
                SaveControlToggle.AddCssClass("dropdown-item dropdown-toggle");
                SaveControlToggle.MergeAttribute("href", "#");
                SaveControlToggle.MergeAttribute("tabindex", "-1");
                //pageSizerToggle.MergeAttribute("data-toggle", "dropdown");
                SaveControlToggle.SetInnerText("Save Filter");
                //TagBuilder rightIcon = new TagBuilder("i");
                //rightIcon.AddCssClass("icon-chevron-right");
                //pageSizerToggle.InnerHtml += rightIcon.ToString();
                saveControl.InnerHtml += SaveControlToggle.ToString();
                TagBuilder saveControlSubContainer = new TagBuilder("ul");
                saveControlSubContainer.AddCssClass("dropdown-menu");
                TagBuilder saveControlSubItem = new TagBuilder("li");
                TagBuilder saveConWrapper = new TagBuilder("form");
                saveConWrapper.AddCssClass("dropdown-item inline-flex");
                saveConWrapper.MergeAttribute("action", url.Action(pageInfo.Settings.GetSaveFilterPage(), MVCVariables.GetRequestControllerName()));
                saveConWrapper.MergeAttribute("method", "get");
                TagBuilder saveTextBoxWrapper = new TagBuilder("span");
                saveTextBoxWrapper.AddCssClass("col-auto");
                TagBuilder saveTextBox = new TagBuilder("input");
                saveTextBox.AddCssClass("search-text form-control");
                saveTextBox.MergeAttribute("name", DataKeys.SaveFilterName);
                saveTextBox.MergeAttribute("placeholder", "Filter Name");
                saveTextBoxWrapper.InnerHtml += saveTextBox.ToString();
                saveConWrapper.InnerHtml += saveTextBoxWrapper.ToString();
                TagBuilder saveButtonWrapper = new TagBuilder("span");
                saveButtonWrapper.AddCssClass("col-auto");
                TagBuilder saveButton = new TagBuilder("button");
                saveButton.AddCssClass("btn btn-info");
                saveButton.MergeAttribute("type", "submit");
                saveButton.MergeAttribute("title", "Save Filter");
                //saveButton.SetInnerText("Save");
                TagBuilder saveButtonIcon = new TagBuilder("i");
                saveButtonIcon.AddCssClass("fas fa-save");
                saveButton.InnerHtml += saveButtonIcon.ToString();
                saveButtonWrapper.InnerHtml += saveButton.ToString();
                saveConWrapper.InnerHtml += saveButtonWrapper.ToString();
                saveControlSubItem.InnerHtml += saveConWrapper.ToString();
                saveControlSubContainer.InnerHtml += saveControlSubItem.ToString();
                saveControl.InnerHtml += saveControlSubContainer.ToString();
                navBarMenu.InnerHtml += saveControl.ToString();
            }
            if (!String.IsNullOrWhiteSpace(menuTemplate))
            {
                navBarMenu.InnerHtml += html.Partial(menuTemplate).ToHtmlString();
            }
            //navBarMenuContainer.InnerHtml += navBarMenu.ToString();
            //container.InnerHtml += navBarMenuContainer.ToString();
            container.InnerHtml += navBarMenu.ToString();
            //navbar.InnerHtml += container.ToString();
            //menuContainer.InnerHtml += navbar.ToString();
            menuContainer.InnerHtml += container.ToString();
            return MvcHtmlString.Create(menuContainer.ToString());
        }

        public static IHtmlString GetTableHeader(this HtmlHelper html, IPageSchema pageInfo, 
            bool ShowExport = false, bool ShowSave = false, bool ShowLoad = false)
        {
            UrlHelper url = pageInfo.GetCurrentUrl();

            TagBuilder tableTag = new TagBuilder("table");
            tableTag.AddCssClass("table-light table-sm table-navigation");
            TagBuilder rowTag = new TagBuilder("tr");

            TagBuilder dividerb = new TagBuilder("td");
            dividerb.InnerHtml += GetInlineTableDivider().ToHtmlString();
            rowTag.InnerHtml += dividerb.ToString();

            TagBuilder recordCol = new TagBuilder("td");
            TagBuilder divider1 = new TagBuilder("td");
            int? totalItemCount = pageInfo.GetCurrentTotalItemCount();
            if (totalItemCount >  0)
            {
                TagBuilder recordWrapper = new TagBuilder("span");
                recordWrapper.AddCssClass("table-navigation-wrap");
                TagBuilder recordCon = new TagBuilder("span");
                recordCon.AddCssClass("col-auto");
                recordCon.SetInnerText($"Found {totalItemCount} records...");
                recordWrapper.InnerHtml += recordCon.ToString();
                recordCol.InnerHtml += recordWrapper.ToString();
                divider1.InnerHtml = GetInlineTableDivider().ToHtmlString();
            }
            else
            {
                recordCol.AddCssClass("table-navigation-empty");
                divider1.AddCssClass("table-navigation-empty");
            }
            rowTag.InnerHtml += recordCol.ToString();
            rowTag.InnerHtml += divider1.ToString();

            TagBuilder pageSizerLabelCol = new TagBuilder("td");
            TagBuilder pageSizerConCol = new TagBuilder("td");
            TagBuilder divider2 = new TagBuilder("td");
            if (pageInfo.CreatePageSizeControl())
            {
                TagBuilder pageSizerLabelWrapper = new TagBuilder("span");
                pageSizerLabelWrapper.AddCssClass("table-navigation-wrap");
                TagBuilder pageSizerLabel = new TagBuilder("span");
                pageSizerLabel.AddCssClass("col-auto");
                pageSizerLabel.SetInnerText("Page Size: ");
                pageSizerLabelWrapper.InnerHtml += pageSizerLabel.ToString();
                pageSizerLabelCol.InnerHtml += pageSizerLabelWrapper.ToString();
                TagBuilder pageSizerWrap = new TagBuilder("form");
                pageSizerWrap.GenerateId("pageSizeForm");
                pageSizerWrap.AddCssClass("table-navigation-wrap");
                pageSizerWrap.MergeAttribute("name", "pageSizeForm");
                pageSizerWrap.MergeAttribute("action", url.Action(pageInfo.Settings.GetSizePageName(), MVCVariables.GetRequestControllerName()));
                pageSizerWrap.MergeAttribute("method", "post");
                pageSizerWrap.GenerateId(DataKeys.PageSize);
                TagBuilder pageSizerCon = new TagBuilder("span");
                pageSizerCon.AddCssClass("col-auto");
                pageSizerCon.InnerHtml += html.DropDownList(DataKeys.PageSize, new SelectList(pageInfo.Settings.GetPageSizeList(), pageInfo.GetCurrentPageSize()), new { @class = "form-control" }).ToHtmlString();
                pageSizerWrap.InnerHtml += pageSizerCon.ToString();
                pageSizerConCol.InnerHtml += pageSizerWrap.ToString();
                divider2.InnerHtml += GetInlineTableDivider().ToHtmlString();
            }
            else
            {
                pageSizerConCol.AddCssClass("table-navigation-empty");
                pageSizerConCol.AddCssClass("table-navigation-empty");
                divider2.AddCssClass("table-navigation-empty");
            }
            rowTag.InnerHtml += pageSizerLabelCol.ToString();
            rowTag.InnerHtml += pageSizerConCol.ToString();
            rowTag.InnerHtml += divider2.ToString();

            TagBuilder exportCol = new TagBuilder("td");
            TagBuilder divider3 = new TagBuilder("td");
            if(ShowExport && pageInfo.CreateExportControl())
            {
                TagBuilder exportWraper = new TagBuilder("span");
                exportWraper.AddCssClass("table-navigation-wrap");
                TagBuilder exportCon = new TagBuilder("a");
                exportCon.AddCssClass("btn btn-info");
                exportCon.MergeAttribute("href", url.Action(pageInfo.Settings.GetExportPageName(), MVCVariables.GetRequestControllerName()));
                exportCon.MergeAttribute("target", "_blank");
                exportCon.SetInnerText("Export");
                exportWraper.InnerHtml += exportCon.ToString();
                exportCol.InnerHtml += exportWraper.ToString();
                divider3.InnerHtml += GetInlineTableDivider().ToHtmlString();
            }
            else
            {
                exportCol.AddCssClass("table-navigation-empty");
                divider3.AddCssClass("table-navigation-empty");
            }
            rowTag.InnerHtml += exportCol.ToString();
            rowTag.InnerHtml += divider3.ToString();

            TagBuilder saveHeaderCol = new TagBuilder("td");
            TagBuilder saveControlCol = new TagBuilder("td");
            TagBuilder divider4 = new TagBuilder("td");
            if(ShowSave && pageInfo.CreateSaveControl())
            {
                TagBuilder saveHeaderWrapper = new TagBuilder("span");
                saveHeaderWrapper.AddCssClass("table-header-wrap");
                TagBuilder saveHeaderCon = new TagBuilder("span");
                saveHeaderCon.AddCssClass("col-auto");
                saveHeaderCon.SetInnerText("Save Filter: ");
                saveHeaderWrapper.InnerHtml += saveHeaderCon.ToString();
                saveHeaderCol.InnerHtml += saveHeaderWrapper.ToString();
                TagBuilder saveConWrapper = new TagBuilder("form");
                saveConWrapper.AddCssClass("table-navigation-wrap");
                saveConWrapper.MergeAttribute("action", url.Action(pageInfo.Settings.GetSaveFilterPage(), MVCVariables.GetRequestControllerName()));
                saveConWrapper.MergeAttribute("method", "post");
                TagBuilder saveTextBoxWrapper = new TagBuilder("span");
                saveTextBoxWrapper.AddCssClass("col-auto");
                TagBuilder saveTextBox = new TagBuilder("input");
                saveTextBox.AddCssClass("search-text form-control");
                saveTextBox.MergeAttribute("name", DataKeys.SaveFilterName);
                saveTextBox.MergeAttribute("placeholder", "Filter Name");
                saveTextBoxWrapper.InnerHtml += saveTextBox.ToString();
                saveConWrapper.InnerHtml += saveTextBoxWrapper.ToString();
                TagBuilder saveButtonWrapper = new TagBuilder("span");
                saveButtonWrapper.AddCssClass("col-auto");
                TagBuilder saveButton = new TagBuilder("button");
                saveButton.AddCssClass("btn btn-info");
                saveButton.MergeAttribute("type", "submit");
                saveButton.MergeAttribute("title", "Load Filter");
                saveButton.SetInnerText("Save");
                saveButtonWrapper.InnerHtml += saveButton.ToString();
                saveConWrapper.InnerHtml += saveButtonWrapper.ToString();
                saveControlCol.InnerHtml += saveConWrapper.ToString();
                TagBuilder reportSavedCon = new TagBuilder("small");
                reportSavedCon.AddCssClass("navigation-col-sub form-text text-muted");
                reportSavedCon.InnerHtml += html.ValidationMessage(DataKeys.SaveFilterName).ToHtmlString();
                saveControlCol.InnerHtml += reportSavedCon.ToString();
                divider4.InnerHtml += GetInlineTableDivider().ToHtmlString();
            }
            else
            {
                saveHeaderCol.AddCssClass("table-navigation-empty");
                saveControlCol.AddCssClass("table-navigation-empty");
                divider4.AddCssClass("table-navigation-empty");
            }
            rowTag.InnerHtml += saveHeaderCol.ToString();
            rowTag.InnerHtml += saveControlCol.ToString();
            rowTag.InnerHtml += divider4.ToString();

            TagBuilder loadHeaderCol = new TagBuilder("td");
            TagBuilder loadControlCol = new TagBuilder("td");
            if(ShowLoad && pageInfo.CreateLoadControl())
            {
                TagBuilder loadHeaderWrapper = new TagBuilder("span");
                loadHeaderWrapper.AddCssClass("table-navigation-wrap");
                TagBuilder loadHeaderCon = new TagBuilder("span");
                loadHeaderCon.AddCssClass("col-auto");
                loadHeaderCon.SetInnerText("Load Filter: ");
                loadHeaderWrapper.InnerHtml += loadHeaderCon.ToString();
                loadHeaderCol.InnerHtml += loadHeaderWrapper.ToString();
                TagBuilder loadConWrapper = new TagBuilder("form");
                loadConWrapper.AddCssClass("table-navigation-wrap");
                loadConWrapper.MergeAttribute("action", url.Action(pageInfo.Settings.GetLoadFilterPageName(), MVCVariables.GetRequestControllerName()));
                loadConWrapper.MergeAttribute("method", "post");
                TagBuilder loadDropDownCon = new TagBuilder("span");
                loadDropDownCon.AddCssClass("col-auto");
                loadDropDownCon.InnerHtml += html.DropDownList(DataKeys.LoadFilterName, new SelectList(pageInfo.Reports), new { @class = "form-control" });
                loadConWrapper.InnerHtml += loadDropDownCon.ToString();
                TagBuilder loadButtonWrapper = new TagBuilder("col-auto");
                loadButtonWrapper.AddCssClass("col-auto");
                TagBuilder loadButton = new TagBuilder("button");
                loadButton.AddCssClass("btn btn-info");
                loadButton.MergeAttribute("type", "submit");
                loadButton.MergeAttribute("title", "Load Filter");
                loadButton.SetInnerText("Load");
                loadButtonWrapper.InnerHtml += loadButton.ToString();
                loadConWrapper.InnerHtml += loadButtonWrapper.ToString();
                loadControlCol.InnerHtml += loadConWrapper.ToString();
                TagBuilder reportLoadCon = new TagBuilder("small");
                reportLoadCon.AddCssClass("navigation-col-sub form-text text-muted");
                reportLoadCon.InnerHtml += html.ValidationMessage(DataKeys.LoadFilterName).ToHtmlString();
                loadControlCol.InnerHtml += reportLoadCon.ToString();
            }
            else
            {
                loadHeaderCol.AddCssClass("table-navigation-empty");
                loadControlCol.AddCssClass("table-navigation-empty");
            }
            rowTag.InnerHtml += loadHeaderCol.ToString();
            rowTag.InnerHtml += loadControlCol.ToString();
            rowTag.InnerHtml += dividerb.ToString();
            tableTag.InnerHtml += rowTag.ToString();

            //TagBuilder row2Tag = new TagBuilder("tr");
            //TagBuilder blankColTag = new TagBuilder("td");
            //blankColTag.AddCssClass("table-navigation-empty");
            //row2Tag.InnerHtml += blankColTag.ToString();
            //row2Tag.InnerHtml += blankColTag.ToString();
            //row2Tag.InnerHtml += blankColTag.ToString();
            //row2Tag.InnerHtml += blankColTag.ToString();
            //row2Tag.InnerHtml += blankColTag.ToString();
            //row2Tag.InnerHtml += blankColTag.ToString();
            //row2Tag.InnerHtml += blankColTag.ToString();
            //TagBuilder reportSavedCol = new TagBuilder("td");
            //if (ShowSave)
            //{
            //    TagBuilder reportSaveWrapper = new TagBuilder("span");
            //    reportSaveWrapper.AddCssClass("navigation-col-sub");
            //    TagBuilder reportSavedCon = new TagBuilder("small");
            //    reportSavedCon.AddCssClass("form-text text-muted");
            //    reportSavedCon.InnerHtml += html.ValidationMessage(DataKeys.SaveFilterName).ToHtmlString();
            //    reportSaveWrapper.InnerHtml += reportSavedCon.ToString();
            //    reportSavedCol.InnerHtml += reportSaveWrapper.ToString();
            //}
            //else
            //{
            //    reportSavedCol.AddCssClass("table-navigation-empty");
            //}
            //row2Tag.InnerHtml += reportSavedCol.ToString();
            //row2Tag.InnerHtml += blankColTag.ToString();
            //row2Tag.InnerHtml += blankColTag.ToString();
            //row2Tag.InnerHtml += blankColTag.ToString();
            //TagBuilder reportLoadCol = new TagBuilder("td");
            //if (ShowLoad)
            //{
            //    TagBuilder reportLoadWrapper = new TagBuilder("span");
            //    reportLoadWrapper.AddCssClass("navigation-col-sub");
            //    TagBuilder reportLoadCon = new TagBuilder("small");
            //    reportLoadCon.AddCssClass("form-text text-muted");
            //    reportLoadCon.InnerHtml += html.ValidationMessage(DataKeys.LoadFilterName).ToHtmlString();
            //    reportLoadWrapper.InnerHtml += reportLoadCon.ToString();
            //    reportLoadCol.InnerHtml += reportLoadWrapper.ToString();
            //}
            //else
            //{
            //    reportLoadCol.AddCssClass("table-navigation-empty");
            //}
            //row2Tag.InnerHtml += reportLoadCol.ToString();
            //tableTag.InnerHtml += row2Tag.ToString();
            return MvcHtmlString.Create(tableTag.ToString());
        }

        public static IHtmlString GetInlineTableDivider()
        {
            TagBuilder deviderCon = new TagBuilder("span");
            deviderCon.AddCssClass("inline-column-divider");
            deviderCon.InnerHtml += "&nbsp;";
            return MvcHtmlString.Create(deviderCon.ToString());
        }

        // Extension method
        public static IHtmlString ActionImage(this HtmlHelper html, string action, object routeValues, string imagePath, string alt, int height = -1, int width = -1)
        {
            var url = new UrlHelper(html.ViewContext.RequestContext);

            // build the <img> tag
            var imgBuilder = new TagBuilder("img");
            imgBuilder.MergeAttribute("src", url.Content(imagePath));
            imgBuilder.MergeAttribute("alt", alt);
            if (height != -1) imgBuilder.MergeAttribute("height", height.ToString());
            if (width != -1) imgBuilder.MergeAttribute("width", width.ToString());
            string imgHtml = imgBuilder.ToString(TagRenderMode.SelfClosing);

            // build the <a> tag
            var anchorBuilder = new TagBuilder("a");
            anchorBuilder.MergeAttribute("href", url.Action(action, routeValues));
            anchorBuilder.InnerHtml = imgHtml; // include the <img> tag inside
            string anchorHtml = anchorBuilder.ToString(TagRenderMode.Normal);

            return MvcHtmlString.Create(anchorHtml);
        }

        public static IHtmlString NumberSpinner<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, int min = 0, int max = 500)
        {
            TagBuilder spinnerWrapper = new TagBuilder("div");
            spinnerWrapper.AddCssClass("input-group number-spinner");
            TagBuilder spinnerDownWrapper = new TagBuilder("span");
            spinnerDownWrapper.AddCssClass("number-spinner-btn data-dwn");
            TagBuilder spinnerDown = new TagBuilder("button");
            spinnerDown.AddCssClass("btn btn-dwn btn-default btn-info");
            spinnerDown.MergeAttribute("data-dir", "dwn");
            TagBuilder spinnerDownIcon = new TagBuilder("i");
            spinnerDownIcon.AddCssClass("fas fa-minus");
            spinnerDown.InnerHtml += spinnerDownIcon.ToString();
            spinnerDownWrapper.InnerHtml += spinnerDown.ToString();
            spinnerWrapper.InnerHtml += spinnerDownWrapper.ToString();
            spinnerWrapper.InnerHtml += html.EditorFor(expression, new { htmlattributes = new { @class = "form-control text-center number-spinner-box", min, max } });
            TagBuilder spinnerUpWrapper = new TagBuilder("span");
            spinnerUpWrapper.AddCssClass("number-spinner-btn data-up");
            TagBuilder spinnerUp = new TagBuilder("button");
            spinnerUp.AddCssClass("btn btn-up btn-default btn-info");
            spinnerUp.MergeAttribute("data-dir", "up");
            TagBuilder spinnerUpIcon = new TagBuilder("i");
            spinnerUpIcon.AddCssClass("fas fa-plus");
            spinnerUp.InnerHtml += spinnerUpIcon.ToString();
            spinnerUpWrapper.InnerHtml += spinnerUp.ToString();
            spinnerWrapper.InnerHtml += spinnerUpWrapper.ToString();
            return MvcHtmlString.Create(spinnerWrapper.ToString());
        }

        public static bool IsKey(this PropertyInfo property)
        {
            if (property == null) return false;
            Type type = property.DeclaringType;
            KeyAttribute attr = (KeyAttribute)property.GetCustomAttributes(typeof(KeyAttribute), true).FirstOrDefault();
            if (attr != null)
                return true;
            var metaDataType = type.GetCustomAttributes(typeof(MetadataTypeAttribute), true).OfType<MetadataTypeAttribute>().FirstOrDefault();
            if (metaDataType != null)
            {
                var metaData = ModelMetadataProviders.Current.GetMetadataForType(null, metaDataType.MetadataClassType);
                return IsKey(metaData.ModelType.GetProperty(property.Name));
                    
            }
            return false;
        }

        public static T GetMetaDataAttribute<T>(this Type model, PropertyInfo property) where T : class
        {
            T retVal = null;
            if (property != null)
            {
                retVal = property.GetCustomAttributes(typeof(T), true).OfType<T>().SingleOrDefault();
                if (retVal == null)
                {
                    var metaDataType = model.GetCustomAttributes(typeof(MetadataTypeAttribute), true).OfType<MetadataTypeAttribute>().FirstOrDefault();
                    if (metaDataType != null)
                    {
                        var metaData = ModelMetadataProviders.Current.GetMetadataForType(null, metaDataType.MetadataClassType);
                        retVal = GetMetaDataAttribute<T>(metaData.ModelType, metaData.ModelType.GetProperty(property.Name));
                    }
                }
            }
            return retVal;
        }

        /// <summary>
        /// Returns the display name of the property given, if no display name given returns property name.
        /// </summary>
        /// <param name="model">Type the property derived from.</param>
        /// <param name="property">Property to get the display name from.</param>
        /// <param name="DisplayNameOnly">If false will return the property name if display name not found. If true and 
        /// display name not found returns null.</param>
        /// <returns></returns>
        public static string GetDisplayName(this Type model, PropertyInfo property, bool DisplayNameOnly = false)
        {
            string retVal = null;
            if (property != null)
            {
                DisplayAttribute attr = GetMetaDataAttribute<DisplayAttribute>(model, property);
                if (attr != null)
                {
                    retVal = attr.Name;
                }
                else
                {
                    DisplayNameAttribute dattr = GetMetaDataAttribute<DisplayNameAttribute>(model, property);
                    if (dattr != null)
                    {
                        retVal = dattr.DisplayName;
                    }
                }
            }
            return retVal ?? (DisplayNameOnly ? null : property.Name);
        }
        
        public static string GetDisplayName<TModel>(Expression<Func<TModel, object>> expression)
        {

            Type type = typeof(TModel);

            string propertyName = null;
            string[] properties = null;
            IEnumerable<string> propertyList;
            //unless it's a root property the expression NodeType will always be Convert
            switch (expression.Body.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    var ue = expression.Body as UnaryExpression;
                    propertyList = (ue?.Operand).ToString().Split(".".ToCharArray()).Skip(1); //don't use the root property
                    break;
                default:
                    propertyList = expression.Body.ToString().Split(".".ToCharArray()).Skip(1);
                    break;
            }

            //the propert name is what we're after
            propertyName = propertyList.Last();
            //list of properties - the last property name
            properties = propertyList.Take(propertyList.Count() - 1).ToArray(); //grab all the parent properties

            Expression expr = null;
            foreach (string property in properties)
            {
                PropertyInfo propertyInfo = type.GetProperty(property);
                expr = Expression.Property(expr, type.GetProperty(property));
                type = propertyInfo.PropertyType;
            }

            DisplayAttribute attr;
            attr = (DisplayAttribute)type.GetProperty(propertyName).GetCustomAttributes(typeof(DisplayAttribute), true).SingleOrDefault();

            // Look for [MetadataType] attribute in type hierarchy
            // http://stackoverflow.com/questions/1910532/attribute-isdefined-doesnt-see-attributes-applied-with-metadatatype-class
            if (attr == null)
            {
                MetadataTypeAttribute metadataType = (MetadataTypeAttribute)type.GetCustomAttributes(typeof(MetadataTypeAttribute), true).FirstOrDefault();
                if (metadataType != null)
                {
                    var property = metadataType.MetadataClassType.GetProperty(propertyName);
                    if (property != null)
                    {
                        attr = (DisplayAttribute)property.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault();
                    }
                }
            }
            return (attr != null) ? attr.Name : String.Empty;
            
        }

        public static string GetDisplayNames(this Type model, bool DisplayNamesOnly, string delimiter = ",")
        {
            Type type = model;
            PropertyInfo[] properties = type.GetProperties();
            string c = ""; StringBuilder retStr = new StringBuilder();
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo prop = properties[i];
                string displayName = type.GetDisplayName(prop, DisplayNamesOnly);
                if (displayName == null && DisplayNamesOnly) continue;
                retStr.Append($"{c}{displayName ?? prop.Name}");
                c = delimiter;
            }
            return retStr.ToString();
        }

        public static IEnumerable<string> GetEnumDisplayNames(this Type enumeration, bool DisplayNameOnly)
        {
            if (!enumeration.IsEnum) throw new NotSupportedException("This method only support Enums.");
            var enums = Enum.GetValues(enumeration);
            List<string> displayNames = new List<string>();
            for (int i = 0; i < enums.Length; i++)
            {
                var e = enums.GetValue(i);
                string name = e.GetEnumDisplayName(DisplayNameOnly);
                if (name == null && DisplayNameOnly) continue;
                displayNames.Add(name);
            }
            return displayNames;
        }

        public static string GetEnumDisplayName(this object enumeration, bool DisplayNameOnly) 
        {
            Type t = enumeration.GetType();
            if (!t.IsEnum) throw new NotSupportedException("This method only support Enums.");
            string name = Enum.GetName(t, enumeration);
            FieldInfo info = t.GetField(name);
            DisplayAttribute attr = (DisplayAttribute)info.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault();
            return (attr != null) ? attr.Name : (DisplayNameOnly) ? null : name;

        }

        public static async Task<string> ToCSVAsync<T>(this IEnumerable array, string delimiter = ",", string newLine = "\r\n")
        {
            return await ToCSVAsync<T>((IEnumerable<T>)array, delimiter, newLine);
        }

        public static async Task<string> ToCSVAsync<T>(this IEnumerable<T> array, string delimiter = ",", string newLine = "\r\n")
        {
            return await Task.Run(() => ToCSV(array, delimiter, newLine));
        }

        public static string ToCSV<T>(this IEnumerable<T> array, string delimiter = ",", string newLine = "\r\n")
        {
            Type type = typeof(T);
            StringBuilder headerStr = new StringBuilder();
            StringBuilder retStr = new StringBuilder();
            string c = "";
            bool firstPass = true;
            foreach(var itm in array)
            {
                foreach(PropertyInfo prop in type.GetProperties())
                {
                    DisplayNameStore store = DisplayNameStore.GetDisplayName(type, prop);
                    if (!store.IsHidden(isExport:true))
                    {
                        if (firstPass)
                        {
                            headerStr.Append($"{c}\"{store.DisplayName}\"");
                        }
                        retStr.Append($"{c}\"{prop.GetValue(itm)}\"");
                        c = delimiter;
                    }
                }
                firstPass = false;
                c = "";
                retStr.Append(newLine);
            }
            return $"{headerStr}{newLine}{retStr}";
        }

        public static async Task<IEnumerable<T>> QueryAllAsync<T>(this IEnumerable<T> model, IEnumerable<IPageFilter> filters)
        {
            return await Task.Run(() => QueryAll(model, filters));
        }


        public static IEnumerable<T> QueryAll<T>(this IEnumerable<T> model, IEnumerable<IPageFilter> filters)
        {
            if (!(filters.Count() > 0)) return model;
            List<T> retList = new List<T>();
            //string cField = null;
            for (int i = 0; i < filters.Count(); i++)
            {
                IPageFilter filter = filters.ElementAt(i);
                if (filter.Field == null)
                {
                    continue;
                }

                //retList.AddRange(Query(model, filter));
                if (filter.Field == DataKeys.AllOption)
                {
                    model = QueryAll(model, filter.Value);
                }
                else
                {
                    model = Query(model, filter);
                }
                if (model == null) return null;
            }
            //return retList.Distinct();
            return model.Distinct();
        }

        public static IEnumerable<T> Query<T>(this IEnumerable<T> model, IPageFilter filter)
        {
            if (filter.Field != null)
            {
                Type type = typeof(T);
                Enum.TryParse(filter.Condition, out PageFilterCondition con);
                PropertyInfo prop = type.GetProperty(filter.Field);
                return Query(model, prop, filter.Value, con);
            }
            return null;
        }

        public static IEnumerable<T> Query<T>(this IEnumerable<T> model, PropertyInfo prop, string value, PageFilterCondition condition)
        {
            StringComparison ignore = StringComparison.CurrentCultureIgnoreCase;

            switch (condition)
            {
                case PageFilterCondition.Eq:
                    model = (from r in model
                             where (prop.GetValue(r) ?? "").ToString().Equals(value ?? "", ignore) == true
                             select r);
                    break;
                case PageFilterCondition.Neq:
                    model = (from r in model
                             where (prop.GetValue(r) ?? "").ToString().Equals(value ?? "", ignore) == false
                             select r);
                    break;
                case PageFilterCondition.Gt:
                    try
                    {
                        if (prop.PropertyType == typeof(Int32) || prop.PropertyType == typeof(Nullable<Int32>))
                        {
                            if (Int32.TryParse(value, out int oVal))
                            {
                                model = (from r in model
                                         where (int?)prop.GetValue(r) > oVal
                                         select r);
                            }
                        }
                        else if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(Nullable<System.DateTime>))
                        {
                            if (DateTime.TryParse(value, out DateTime oVal))
                            {
                                model = (from r in model
                                         where (DateTime?)prop.GetValue(r) > oVal
                                         select r);
                            }
                        }
                        else
                        {
                            model = (from r in model
                                     where prop.GetValue(r).ToString().ConvertToASCIISum() > (value ?? String.Empty).ConvertToASCIISum()
                                     select r);
                        }
                    }
                    catch (Exception)
                    {
                        try
                        {
                            model = (from r in model
                                     where (prop.GetValue(r) ?? String.Empty).ToString().ConvertToASCIISum() > (value ?? String.Empty).ConvertToASCIISum()
                                     select r);
                        }
                        catch (Exception)
                        { }
                    }
                    break;
                case PageFilterCondition.Ge:
                    try
                    {
                        if (prop.PropertyType == typeof(Int32) || prop.PropertyType == typeof(Nullable<Int32>))
                        {
                            if (Int32.TryParse(value, out int oVal))
                            {
                                model = (from r in model
                                         where (int?)prop.GetValue(r) >= oVal
                                         select r);
                            }
                            else if(prop.PropertyType == typeof(Nullable<Int32>) && value == null)
                            {
                                model = (from r in model
                                         where (int?)prop.GetValue(r) == null
                                         select r);
                            }
                        }
                        else if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(Nullable<System.DateTime>))
                        {
                            if (DateTime.TryParse(value, out DateTime oVal))
                            {
                                model = (from r in model
                                         where (DateTime?)prop.GetValue(r) >= oVal
                                         select r);
                            }
                            else if(prop.PropertyType == typeof(Nullable<System.DateTime>) && value == null)
                            {
                                model = (from r in model
                                         where (DateTime?)prop.GetValue(r) == null
                                         select r);
                            }
                        }
                        else
                        {
                            model = (from r in model
                                     where (prop.GetValue(r) ?? String.Empty).ToString().ConvertToASCIISum() >= (value ?? String.Empty).ConvertToASCIISum()
                                     select r);
                        }
                    }
                    catch (Exception)
                    {
                        try
                        {
                            model = (from r in model
                                     where (prop.GetValue(r) ?? String.Empty).ToString().ConvertToASCIISum() >= (value ?? String.Empty).ConvertToASCIISum()
                                     select r);
                        }
                        catch (Exception)
                        { }
                    }
                    break;
                case PageFilterCondition.Lt:
                    try
                    {
                        if (prop.PropertyType == typeof(Int32) || prop.PropertyType == typeof(Nullable<Int32>))
                        {
                            if (Int32.TryParse(value, out int oVal))
                            {
                                model = (from r in model
                                         where (int?)prop.GetValue(r) < oVal
                                         select r);
                            }
                        }
                        else if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(Nullable<System.DateTime>))
                        {
                            if (DateTime.TryParse(value, out DateTime oVal))
                            {
                                model = (from r in model
                                         where (DateTime?)prop.GetValue(r) < oVal
                                         select r);
                            }
                        }
                        else
                        {
                            model = (from r in model
                                     where (prop.GetValue(r) ?? String.Empty).ToString().ConvertToASCIISum() < (value ?? String.Empty).ConvertToASCIISum()
                                     select r);
                        }
                    }
                    catch (Exception)
                    {
                        try
                        {
                            model = (from r in model
                                     where (prop.GetValue(r) ?? String.Empty).ToString().ConvertToASCIISum() < (value ?? String.Empty).ConvertToASCIISum()
                                     select r);
                        }
                        catch (Exception)
                        { }
                    }
                    break;
                case PageFilterCondition.Le:
                    try
                    {
                        if (prop.PropertyType == typeof(Int32) || prop.PropertyType == typeof(Nullable<Int32>))
                        {
                            if (Int32.TryParse(value, out int oVal))
                            {
                                model = (from r in model
                                         where (int?)prop.GetValue(r) <= oVal
                                         select r);
                            }
                            else if (prop.PropertyType == typeof(Nullable<Int32>) && value == null)
                            {
                                model = (from r in model
                                         where (int?)prop.GetValue(r) == null
                                         select r);
                            }
                        }
                        else if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(Nullable<System.DateTime>))
                        {
                            if (DateTime.TryParse(value, out DateTime oVal))
                            {
                                model = (from r in model
                                         where (DateTime?)prop.GetValue(r) <= oVal
                                         select r);
                            }
                            else if (prop.PropertyType == typeof(Nullable<System.DateTime>) && value == null)
                            {
                                model = (from r in model
                                         where (DateTime?)prop.GetValue(r) == null
                                         select r);
                            }
                        }
                        else
                        {
                            model = (from r in model
                                     where (prop.GetValue(r) ?? String.Empty).ToString().ConvertToASCIISum() <= (value ?? String.Empty).ConvertToASCIISum()
                                     select r);
                        }
                    }
                    catch (Exception)
                    {
                        try
                        {
                            model = (from r in model
                                     where (prop.GetValue(r) ?? String.Empty).ToString().ConvertToASCIISum() <= (value ?? String.Empty).ConvertToASCIISum()
                                     select r);
                        }
                        catch (Exception)
                        { }
                    }
                    break;
                case PageFilterCondition.Like:
                    model = (from r in model
                             where (prop.GetValue(r) ?? String.Empty).ToString().IndexOf(value ?? String.Empty, ignore) >= 0
                             select r);
                    break;
                case PageFilterCondition.NotLike:
                    model = (from r in model
                             where (prop.GetValue(r) ?? String.Empty).ToString().IndexOf(value ?? String.Empty, ignore) < 0
                             select r);
                    break;
                default:
                    break;
            }
            return model;
        }

        public static async Task<IEnumerable<T>> QueryAllAsync<T>(this IEnumerable<T> model, string query)
        {
            return await Task.Run(() => QueryAll(model, query));
        }

        public static IEnumerable<T> QueryAll<T>(this IEnumerable<T> model, string query)
        {
            if (query == null) return model;
            Type type = typeof(T);
            PropertyInfo[] props = type.GetProperties();
            List<T> retList = new List<T>();
            StringComparison ignore = StringComparison.CurrentCultureIgnoreCase;

            int propCount = props.Count();
            for (int i = 0; i < propCount; i++)
            {
                PropertyInfo prop = props.ElementAt(i);
                retList.AddRange((from r in model
                         where prop.GetValue(r) != null ? prop.GetValue(r).ToString().IndexOf(query, ignore) >= 0 : false
                         select r));
            }
            return retList;
        }



        public static bool RouteExists(this HtmlHelper html, string Controller, string Action)
        {
            if (Action == null)
            {
                string filePath = $"Views/{Controller}/";
                var absolutePath = HttpContext.Current.Server.MapPath("~/" + filePath);
                return Directory.Exists(absolutePath);
            }
            else
            {
                string filePath = $"Views/{Controller}/{Action}.cshtml";
                var absolutePath = HttpContext.Current.Server.MapPath("~/" + filePath);
                return File.Exists(absolutePath);

            }
        }
    }

    internal class DisplayNameStore
    {
        private static List<DisplayNameStore> _store = new List<DisplayNameStore>();

        public string DisplayName { get; }
        public Type ModelType { get; }
        public PropertyInfo Property { get; }

        private DisplayNameStore(string displayName, Type modelType, PropertyInfo prop)
        {
            ModelType = modelType;
            DisplayName = displayName;
            Property = prop;
        }
        public static DisplayNameStore GetDisplayName(Type type, PropertyInfo prop)
        {
            DisplayNameStore foundStore = _store.Find(x => x.Property == prop);
            if (foundStore != null) return foundStore;
            string displayName = type.GetDisplayName(prop, false);
            foundStore = new DisplayNameStore(displayName, type, prop);
            _store.Add(foundStore);
            return foundStore;
        }

        public bool IsHidden(bool isTable = false, bool isList = false, bool isExport = false)
        {
            var att = ModelType.GetMetaDataAttribute<HideAttribute>(Property);
            if (isTable)
                return att?.ShowTable == false;
            else if (isList)
                return att?.ShowList == false;
            else if (isExport)
                return att?.ShowExport == false;
            else
                return att != null;
        }
    }

    [Serializable]
    public class ModelException : Exception
    {
        public string Model { get; set; }

        public ModelException(string model) { setModel(model, ""); }
        public ModelException(string message, string model) : base(message) { setModel(model, message); }

        public ModelException(string message, string model, Exception inner) : base(message, inner) { setModel(model, message); }
        protected ModelException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }

        internal void setModel(string model, string message)
        {
            Model = model;
        }
    }

    public static class LabelExtensions
    {
        public static IHtmlString UnencodedLabelFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            var htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            var text = (metadata.DisplayName ?? (metadata.PropertyName ?? htmlFieldName.Split(new char[] { '.' }).Last<string>()));
            if (string.IsNullOrEmpty(text))
            {
                return MvcHtmlString.Empty;
            }
            var tagBuilder = new TagBuilder("label");
            tagBuilder.Attributes.Add("for", TagBuilder.CreateSanitizedId(html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName)));
            tagBuilder.InnerHtml = text;
            return new HtmlString(tagBuilder.ToString(TagRenderMode.Normal));
        }

        //public static IHtmlString UnencodedLabel<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression)
        //{
        //    var metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
        //    var htmlFieldName = ExpressionHelper.GetExpressionText(expression);
        //    var text = (metadata.DisplayName ?? (metadata.PropertyName ?? htmlFieldName.Split(new char[] { '.' }).Last<string>()));
        //    if (string.IsNullOrEmpty(text))
        //    {
        //        return MvcHtmlString.Empty;
        //    }
        //    var tagBuilder = new TagBuilder("label");
        //    //tagBuilder.Attributes.Add("for", TagBuilder.CreateSanitizedId(html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName)));
        //    tagBuilder.InnerHtml = text;
        //    return new HtmlString(tagBuilder.ToString(TagRenderMode.Normal));
        //}
    }
 } 
