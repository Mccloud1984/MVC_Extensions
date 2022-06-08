using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MVC.Extensions.DataAnnotations
{
    /// <summary>
    /// For a boolean field, set the display text for <c>true</c> and
    /// <c>false</c> values.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property,
                    Inherited = false, AllowMultiple = false)]
    public class BooleanDisplayValuesAttribute : Attribute, IMetadataAware
    {
        public const string TrueTitleAdditionalValueName = "BooleanTrueValueTitle";
        public const string FalseTitleAdditionalValueName = "BooleanFalseValueTitle";

        private readonly string _trueValueTitle;
        private readonly string _falseValueTitle;

        public BooleanDisplayValuesAttribute(string trueValueTitle,
                                             string falseValueTitle)
        {
            _trueValueTitle = trueValueTitle;
            _falseValueTitle = falseValueTitle;
        }

        public void OnMetadataCreated(ModelMetadata metadata)
        {
            metadata.AdditionalValues[TrueTitleAdditionalValueName] = _trueValueTitle;
            metadata.AdditionalValues[FalseTitleAdditionalValueName] = _falseValueTitle;
        }
    }

}
