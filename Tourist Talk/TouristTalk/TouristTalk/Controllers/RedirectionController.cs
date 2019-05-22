using System;
using System.Web.Mvc;

namespace TouristTalk.Controllers
{
   /// <summary>
   /// Controller used for redirection
   /// </summary>
    public class RedirectionController : Controller
    {
        
        /// <summary>
        /// Sets where to redirect to if user not authorised for current page
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public ActionResult NotAuthorisedRedirect()
        {
            try
            {
                return RedirectToAction("Login", "Account");
            }
            catch (Exception e)
            {
                return RedirectToAction("HandledCodeError", "ErrorHandler", new { exception = e.ToString() });
            }
        }
    }
}