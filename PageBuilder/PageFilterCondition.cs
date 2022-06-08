using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MVC.Extensions
{

    public enum PageFilterCondition
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

}