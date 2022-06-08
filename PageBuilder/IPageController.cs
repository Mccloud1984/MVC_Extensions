using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MVC.Extensions.PageBuilder
{
    interface IPageController
    {
        Task<ActionResult> Index(int page = 1);
        ActionResult Clear();
        Task<ActionResult> Details(object id = null);
        ActionResult Sort(string sortOrder);
        ActionResult PageSize(int pageSize);
        Task<ActionResult> Export();
        ActionResult Create();
        ActionResult Filter(IEnumerable<PageFilter> filters = null);
        Task<ActionResult> Edit(object id);
        Task<ActionResult> Delete(object id);

    }
}
