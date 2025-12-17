using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Voyage.Utilities
{
    public class CustomAttributes
    {
        public class ValidateHeaderAntiForgeryTokenAttribute : TypeFilterAttribute
        {
            public ValidateHeaderAntiForgeryTokenAttribute() : base(typeof(ValidateHeaderAntiForgeryTokenFilter))
            {
            }

            private class ValidateHeaderAntiForgeryTokenFilter : IAsyncAuthorizationFilter
            {
                public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
                {
                    var antiforgery = context.HttpContext.RequestServices.GetService<IAntiforgery>();
                    await antiforgery.ValidateRequestAsync(context.HttpContext);
                }
            }
        }
    }
}
