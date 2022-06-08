using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using CMWME.C_Extensions;
using MVC.Extensions.HtmlHelpers;

namespace MVC.Extensions.Table
{
    public class TableColumn<TModel, TProperty> : ITableColumn, ITableColumnInternal<TModel> where TModel : class
    {
        public object HtmlAttributes { get; }
        public string TemplateName { get; private set; }

        public string ColumnName { get; }
        /// <summary>
        /// Compiled lambda expression to get the property value from a model object.
        /// </summary>
        public Expression<Func<TModel, TProperty>> Expression { get; set; }

        internal TableColumn()
        {
            TemplateName = null;
            ColumnName = null;
            Expression = null;
            HtmlAttributes = null;
        }
        public void SetTemplate()
        {
            SetTemplate(ColumnName);
        }

        public void SetTemplate(string templateName)
        {
            TemplateName = templateName;
        }

        public string GetDisplayFor(HtmlHelper<TModel> html, TModel model, object htmlAttributes = null)
        {
            //html.ViewData.Add( new KeyValuePair<string, object>( ViewDataKeys.CurrentModel, model));
            //html.ViewData.Add(new KeyValuePair<string, object>( ViewDataKeys.PropertyName, ColumnName));
            object mergedHtmlAttributes = html.MergeAnonymousTypes(HtmlAttributes, htmlAttributes);
            return html.DisplayFor(Expression, TemplateName ?? ColumnName, ColumnName,
                new { ModelInfo = new ModelInfo(model, ColumnName), htmlAttributes = mergedHtmlAttributes }).ToHtmlString();
        }

        public string GetEditorFor(HtmlHelper<TModel> html, TModel model, object htmlAttributes = null)
        {
            object mergedHtmlAttributes = html.MergeAnonymousTypes(HtmlAttributes, htmlAttributes);

            if (GetIsEnum() && string.IsNullOrWhiteSpace(TemplateName))
            {
                return EnumDropDownListFor(html, model, mergedHtmlAttributes);
            }
            else
            {
                string retVal = "";
                if(GetIsReadonly() || !GetIsEditable())
                {
                    retVal += html.HiddenFor(Expression);
                }
                retVal += html.EditorFor(Expression, TemplateName ?? ColumnName, ColumnName,
                    new { ModelInfo = new ModelInfo(model, ColumnName), htmlAttributes = mergedHtmlAttributes }).ToHtmlString();
                return retVal;
            }
        }

        private bool GetIsEditable()
        {
            return typeof(TModel).GetMetaDataAttribute<EditableAttribute>(typeof(TModel).GetProperty(ColumnName))?.AllowEdit ?? true;
        }

        private bool GetIsReadonly()
        {
            return typeof(TModel).GetMetaDataAttribute<ReadOnlyAttribute>(typeof(TModel).GetProperty(ColumnName))?.IsReadOnly ?? false;
        }

        public bool GetIsEnum()
        {
            return typeof(TProperty).IsEnum;
        }

        public string EnumDropDownListFor(HtmlHelper<TModel> html, TModel model, object htmlAttributes = null)
        {
            htmlAttributes = html.MergeAnonymousTypes(HtmlAttributes, htmlAttributes);//, new { name = ColumnName });
            return html.EnumDropDownListFor(Expression, htmlAttributes as IDictionary<string, object>).ToHtmlString(); //, htmlAttributes
        }

        public string GetHiddenFor(HtmlHelper<TModel> html, TModel model) 
        {
            return html.HiddenFor(Expression).ToHtmlString();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="expression">Lambda expression identifying a property to be rendered.</param>
        public TableColumn(Expression<Func<TModel, TProperty>> expression, object htmlAttributes = null)
        {
            ColumnName = (expression.Body as MemberExpression).Member.Name;
            //this.ColumnTitle = Regex.Replace(propertyName, "([a-z])([A-Z])", "$1 $2");
            this.Expression = expression;
            this.HtmlAttributes = htmlAttributes;
        }
    }
}
