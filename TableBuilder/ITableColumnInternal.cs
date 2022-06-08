using System.Web.Mvc;

namespace MVC.Extensions.Table
{
    /// <summary>
    /// Properties and methods used within the TableBuilder class.
    /// </summary>
    public interface ITableColumnInternal<TModel> where TModel : class
    {
        //string ColumnTitle { get; set; }
        //string Evaluate(TModel model);
        string ColumnName { get; }

        string GetDisplayFor(HtmlHelper<TModel> html, TModel model, object htmlAttributes = null);

        string GetEditorFor(HtmlHelper<TModel> html, TModel model, object htmlAttributes = null);

        bool GetIsEnum();
        string EnumDropDownListFor(HtmlHelper<TModel> html, TModel model, object htmlAttributes = null);
        string GetHiddenFor(HtmlHelper<TModel> html, TModel model);
    }
}
