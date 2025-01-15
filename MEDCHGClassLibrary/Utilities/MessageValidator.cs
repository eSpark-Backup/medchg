using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.Reflection;


namespace MEDCHGClassLibrary.Utilities
{
   
    public static class PropertyBuilderExtensions
    {
        public static PropertyBuilder<TProperty> HasErrorMessage<TProperty>(this PropertyBuilder<TProperty> builder, string errorMessage)
        {
            var property = builder.Metadata;
            var requiredAttribute = property.PropertyInfo.GetCustomAttribute<RequiredAttribute>();
            if (requiredAttribute != null)
            {
                requiredAttribute.ErrorMessage = errorMessage;
            }
            return builder;
        }
    }
}
