using X.PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MVC.Extensions.Table
{
    public class ColumnBuilder<TModel> where TModel : class
    {
        public TableBuilder<TModel> TableBuilder { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="tableBuilder">Instance of a TableBuilder.</param>
        public ColumnBuilder(TableBuilder<TModel> tableBuilder)
        {
            TableBuilder = tableBuilder;
        }

        /// <summary>
        /// Add lambda expressions to the TableBuilder.
        /// </summary>
        /// <typeparam name="TProperty">Class property that is rendered in the column.</typeparam>
        /// <param name="expression">Lambda expression identifying a property to be rendered.</param>
        /// <returns>An instance of TableColumn.</returns>
        public ITableColumn SetColumn<TProperty>(Expression<Func<TModel, TProperty>> expression, object htmlAttributes = null)
        {
            return TableBuilder.AddColumn(expression, htmlAttributes);
        }
    }
}
