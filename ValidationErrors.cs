using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace MVC.Extensions
{
    public static class ValidationErrorContext
    {
        public static bool IsValid { get { return _isValid; } }
        internal static bool _isValid = true;

        public static ReadOnlyCollection<ValidationError> ValidationErrors { get { return new ReadOnlyCollection<ValidationError>(_validationErrors); } }
        internal static Collection<ValidationError> _validationErrors = new Collection<ValidationError>();
        
        public static void AddValidationError(string Message, string Model)
        {
            AddValidationError(Message, Model, null);
        }

        public static void AddValidationError(string Message, string Model, params object[] modelStringFormats)
        {
            if (modelStringFormats != null && modelStringFormats.Length > 0) { Model = String.Format(Model, modelStringFormats); }
            _validationErrors.Add(new ValidationError() { ErrorMessage = Message, Model = Model });
            _isValid = false;
        }

        public static string PrintErrors()
        {
            StringBuilder errors = new StringBuilder();
            try
            {
                int length = _validationErrors.Count;
                for (int i = 0; i < length; i++)
                {
                    ValidationError error = _validationErrors[i];
                    errors.AppendFormat("Property: {0}\nError: {1}\n\n", error.Model, error.ErrorMessage);
                }
                return errors.ToString();
            }
            catch (Exception ex)
            {
                return String.Format("Error Printing Errors. Error: {0}", ex.Message);
            }
        }
    }

    public class ValidationError
    {
        public string ErrorMessage { get; set; }
        public string Model { get; set; }

        internal ValidationError() { }
    }
}
