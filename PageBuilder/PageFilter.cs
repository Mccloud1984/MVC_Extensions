using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC.Extensions
{
    public partial class PageFilter : IPageFilter
    {
        public enum FilterCondition
        {
            [Display(Name = "Equals")]
            Eq,
            [Display(Name = "Not Equals")]
            Neq,
            [Display(Name = "Greater Than")]
            Gt,
            [Display(Name = "Greater Than Or Equals")]
            Ge,
            [Display(Name = "Less Than")]
            Lt,
            [Display(Name = "Less Than Or Equals")]
            Le,
            [Display(Name = "Like")]
            Like,
            [Display(Name = "Not Like")]
            NotLike
        }
        public int FilterId { get; set; }
        public string PageName { get; set; }
        public string Report { get; set; }
        public string Field { get; set; }
        public string Condition { get; set; }
        public string Value { get; set; }
    }
}
