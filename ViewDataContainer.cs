using System.Web.Mvc;

namespace MVC.Extensions
{

    public class ViewDataContainer<TModel>
        : IViewDataContainer
    {
        public ViewDataContainer(TModel model)
        {
            ViewData = new ViewDataDictionary<TModel>(model);
        }

        public ViewDataDictionary ViewData { get; set; }
    }

}