using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LogCentre.Web.Helpers
{
    public static class ModelStateHelper
    {
        public static string GetModelStateErrors(ModelStateDictionary state)
        {
            if (state.IsValid) return string.Empty;
            var errors = state.Values
                .SelectMany(state => state.Errors)
                .Select(error => error.ErrorMessage);

            var errorString = string.Join(", ", errors);
            return errorString;
        }
    }
}