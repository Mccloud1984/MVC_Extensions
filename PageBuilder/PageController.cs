using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MVC.Extensions.PageBuilder
{
    public abstract class PageController<TModel> : Controller, IPageController where TModel: PageData
    {

        public static PageSchema<TModel> Page { get; set; }

        public virtual async Task<ActionResult> Index(int page = 1)
        {
            MergeModelState();

            if(!(Page.GetModelCount() > 0))
            {
                IEnumerable<TModel> query = await Page.GetDBList();
                Page.SetBaseModel(query);
            }
            var queryPaged = await Page.GetPagedListAsync(page: page);
            return View(Page.Settings.GetIndexPageName(), queryPaged);
        }

        public virtual ActionResult Clear()
        {
            Page.ClearSession();
            return RedirectToAction(Page.Settings.GetIndexPageName());
        }

        public virtual async Task<ActionResult> Details(object id = null)
        {
            if (id == null) return HttpNotFound();
            TModel model = await Page.GetModelById(id);
            return View(Page.Settings.GetDetailsPageName(), model);
        }
         
        public virtual ActionResult PageSize(int pageSize)
        {
            Page.SetCurrentPageSize(pageSize);
            return RedirectToAction(Page.Settings.GetIndexPageName());
        }

        public virtual ActionResult Sort(string sortOrder)
        {
            Page.SetCurrentSort(sortOrder);
            return RedirectToAction(Page.Settings.GetIndexPageName());
        }

        public virtual async Task<ActionResult> Export()
        {
            return await Page.ToCSV();
        }


        // GET: Create
        public virtual ActionResult Create()
        {
            return View(Page.Settings.GetCreatePageName());
        }

        // POST: Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Create(TModel model)
        {
            try
            {
                MergeModelState();
                if (ModelState.IsValid)
                {
                    await Page.AddModelAsync(model);
                    return RedirectToAction(Page.Settings.GetIndexPageName());
                }
            }
            catch (Exception ex)
            {
                AddExceptionToModelState<TModel>(ex);
            }
            return RedirectToAction(Page.Settings.GetCreatePageName(), model);
        }

        public virtual ActionResult Filter(IEnumerable<PageFilter> filters = null)
        {
            if(filters != null) Page.SetCurrentQuery(filters);
            //ViewData["fieldType"] = typeof(TModel);
            return RedirectToAction(Page.Settings.GetIndexPageName());
        }

        public void MergeModelState()
        {
            if (TempData["ModelState"] != null)
                ModelState.Merge((ModelStateDictionary)TempData["ModelState"]);
        }

        public void AddExceptionToModelState<T>(Exception ex)
        {
            ModelState.AddModelError(typeof(T).Name, GetInternalException(ex)?.Message);
            TempData["ModelState"] = ModelState;
        }

        [HttpGet]
        public virtual async Task<ActionResult> Edit(object id)
        {
            if (id == null) return HttpNotFound();
            MergeModelState();
            TModel model = await Page.GetModelById(id);
            return View(Page.Settings.GetEditPageName(), model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Edit(TModel model)
        {
            try
            {
                MergeModelState();
                if (!Page.CanEdit(User))
                    return RedirectToAction(Page.Settings.GetIndexPageName());
                if (ModelState.IsValid)
                {
                    await Page.SaveModelAsync(model);
                    return RedirectToAction(Page.Settings.GetIndexPageName());
                }
            }
            catch (Exception ex)
            {
                AddExceptionToModelState<TModel>(ex);
            }
            return View(Page.Settings.GetEditPageName(), model);
        }

        public Exception GetInternalException(Exception ex)
        {
            return ex.InnerException != null ? GetInternalException(ex.InnerException) : ex;
        }


        // GET: ButtonRequestSets/Delete/5
        public virtual async Task<ActionResult> Delete(object id)
        {
            if (!Page.CanEdit(User))
                return RedirectToAction(Page.Settings.GetIndexPageName());
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            TModel model = await Page.GetModelById(id);
            if (model == null)
                return HttpNotFound();
            return View(Page.Settings.GetIndexPageName(), model);
        }

        // POST: ButtonRequestSets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> DeleteConfirmed(object id)
        {
            if (!Page.CanEdit(User))
                return RedirectToAction(Page.Settings.GetIndexPageName());
            TModel model = await Page.GetModelById(id);
            //db.ButtonRequestSets.Remove(buttonRequestSet);
            //await db.SaveChangesAsync();
            await Page.DeleteModelAsync(model);
            return RedirectToAction(Page.Settings.GetIndexPageName());
        }

        protected override void Dispose(bool disposing)
        {
            Page?.Dispose(disposing);
            base.Dispose(disposing);
        }
    }
}
