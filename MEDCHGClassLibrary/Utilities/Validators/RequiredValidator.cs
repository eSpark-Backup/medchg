using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEDCHGClassLibrary.Utilities.Validators
{
    public class RequiredValidator : ICustomValidator
    {
        private string PropertyName { get; set; }

        private bool IsRequired { get; set; }
        public RequiredValidator(bool _IsRequired ,string _PropertyName)
        {
            IsRequired = _IsRequired;
            PropertyName = _PropertyName;
        }

        public string ErrorMessage { get; set; } = string.Empty;

        public bool IsValid(object value)
        {
            // Check if the value is a string
            if (value is string || value is null)
            {
                
                if (string.IsNullOrEmpty((string)value))
                {
                    ErrorMessage = $"{PropertyName} is required";
                    // Return true if the validation passes
                    return false;
                }

            }

            // Return false if the validation fails
            return true;
        }
    }
}
