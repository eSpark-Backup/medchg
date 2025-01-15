using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MEDCHGClassLibrary.Utilities.Validators
{
    public class LengthCheckValidator : ICustomValidator
    {

        // Define a property to store the maximum length

        private int MaxLength { get; set; }

        private int MinLength { get; set; }

        public string? ErrorMessage { get; set; } = " ";

        private string PropertyName { get; set; }




        public LengthCheckValidator(int _MinLength, int _MaxLength, string _PropertyName)
        {
            MinLength = _MinLength;
            MaxLength = _MaxLength;
            PropertyName = _PropertyName;
        }


        // Override the IsValid method
        public bool IsValid(object value)
        {
            if (value is null)
            {
                value = "";
            }
            // Check if the value is a string
            if (value is string)
            {
                // Get the length of the string
                int length = ((string)value).Length;

                // Check if the length is less than or equal to the maximum length
                if (length < MinLength || length > MaxLength)
                {
                    ErrorMessage = $"{PropertyName} must be between {MinLength} and {MaxLength} characters.";
                    // Return true if the validation passes
                    return false;
                }

            }

            // Return false if the validation fails
            return true;
        }
    }

}

