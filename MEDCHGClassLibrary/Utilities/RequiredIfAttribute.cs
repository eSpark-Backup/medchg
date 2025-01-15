using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEDCHGClassLibrary.Utilities
{
    public class RequiredIfAttribute : RequiredAttribute
    {
        public string PropertyName { get; private set; }
        public object PropertyValue { get; private set; }

        public RequiredIfAttribute(string propertyName, object propertyValue)
        {
            PropertyName = propertyName;
            PropertyValue = propertyValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var property = context.ObjectType.GetProperty(PropertyName);
            if (property == null)
            {
                return new ValidationResult($"Property {PropertyName} not found.");
            }

            var propertyValue = property.GetValue(context.ObjectInstance);

            // Only validate if the property value matches the expected value
            if (propertyValue.Equals(PropertyValue))
            {
                return base.IsValid(value, context);
            }

            // Otherwise, return success
            return ValidationResult.Success;
        }
    }
}
