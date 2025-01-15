using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEDCHGClassLibrary.Utilities.Validators
{
    public class ValueChecker : ICustomValidator
    {

        private  int MaxValue { get; set; }

        private  string PropertyName { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;

        public ValueChecker(int _MaxValue , string _PropertyName)
        {
            MaxValue = _MaxValue;
            PropertyName = _PropertyName;
        }


        // Override the IsValid method
        public  bool IsValid(object value)
        {
            // Check if the value is a string

            if (value == null)
            {
                return false; 
            }

           else if (value is int)
            {
                // Get the length of the string
               

                // Check if the length is less than or equal to the maximum length
                if ((int)value < 1 || (int)value > MaxValue)
                {
                    ErrorMessage = $"{PropertyName} value must be between {1} and {MaxValue} characters.";
                    // Return true if the validation passes
                    return false;
                }

            }

            // Return false if the validation fails
            return true;
        }
    }
}

