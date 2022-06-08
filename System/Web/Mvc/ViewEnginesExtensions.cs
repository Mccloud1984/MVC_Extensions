using CMWME.C_Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Mvc
{
    public static class ViewEnginesExtensions
    {
        public static string PartialName = "Partials";
        /// <summary>
        /// Modifies the PartialViewLocationFormats of the razor view engine adding additional locations to each location format<br />
        /// that replaces the {0} with Partials/{0}. With this you can create a Partials folder under the view folders to store your <br />
        /// partials in. The folder value name Partials can be changed by changing the variable ViewEnginesExtenstions.PartialName.
        /// </summary>
        public static void AddPartialViewEngines(this ViewEngineCollection engines)
        {
            //engine.Clear();
            foreach(var e in engines)
            {
                if(e.IsType(out RazorViewEngine rzrEngine))
                {
                    List<string> pList = new List<string>();
                    foreach(var r in rzrEngine.PartialViewLocationFormats)
                    {
                        pList.Add(r);
                        pList.Add(r.Replace("{0}", PartialName + "/{0}"));
                    }
                    rzrEngine.PartialViewLocationFormats = pList.ToArray();
                }
            }
            //var customEngine = new RazorViewEngine
            //{
            //    PartialViewLocationFormats = new string[]
            //    {
            //        "~/Views/{1}/{0}.cshtml",
            //        "~/Views/Shared/{0}.cshtml",
            //        "~/Views/Shared/Partials/{0}.cshtml",
            //        "~/Views/{1}/Partials/{0}.cshtml"
            //    },
            //    ViewLocationFormats = new string[]
            //    {
            //        "~/Views/{1}/{0}.cshtml",
            //        "~/Views/Shared/{0}.cshtml",
            //        "~/Views/Controller/{1}/{0}.cshtml"
            //    },
            //    MasterLocationFormats = new string[]
            //    {
            //        "~/Views/Shared/{0}.cshtml",
            //        "~/Views/Shared/Layouts/{0}.cshtml"
            //    }
            //};

            //engine.Add(customEngine);
        }
    }
}
