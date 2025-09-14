using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Post.Common.Utilities
{
    public static class ValidationExtension
    {
        public static void AddToModelState(this FluentValidation.Results.ValidationResult result, ModelStateDictionary modelState)
        {
            foreach (var error in result.Errors)
            {
                modelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
        }
    }
}
