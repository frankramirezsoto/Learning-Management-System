using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using CanvasLMS.Extensions;
using CanvasLMS.Models.Entities;

namespace CanvasLMS.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequireSessionAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Check if the required session object exists in the session
            if (context.HttpContext.Session.GetString("Professor") == null && context.HttpContext.Session.GetString("Student") == null)
            {
                // Redirect to the home screen or login page
                context.Result = new RedirectResult("~/");
            }
        }
    }
}
