using System.Web.Mvc;
using TouristTalk.Models;
using TouristTalk.Controllers;

namespace TouristTalk.Validation
{
    /// <summary>
    /// Custom attribute for ensuring user authenticated
    /// </summary>
    public class AuthorizeUserAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!SessionHandler.Authenticated)
            {
                filterContext.Result = (new RedirectionController()).NotAuthorisedRedirect();
            }
        }

    }
}
