using Microsoft.AspNetCore.Mvc.ModelBinding;

using System.Text;

namespace LogCentre.Web.Helpers
{
    public static class ModelStateHelper
    {
        public static string GetModelStateErrors(ModelStateDictionary state)
        {
            if (state.IsValid) return string.Empty;
            var errors = state.Where(x => x.Value.Errors.Count > 0)
                .Select(x => new { x.Key, x.Value.Errors })
                .ToArray();
            if (errors.Length == 0)
            {
                return string.Empty;
            }
            var errorString = new StringBuilder();
            foreach (var error in errors)
            {
                foreach (var modelError in error.Errors)
                {
                    if (errorString.Length > 0)
                    {
                        errorString.Append(", ");
                    }

                    errorString.Append(modelError.ErrorMessage);
                }
            }

            return errorString.ToString();
        }
    }
}
