using System;
using System.Web.Mvc;

namespace TouristTalk.Controllers
{
    /// <summary>
    /// Controller for Home page
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// returns home page
        /// </summary>
        /// <returns>home page</returns>
        public ActionResult About()
        {   
            try
            {
                return View();
            }
            catch (Exception e)
            {
                return RedirectToAction("HandledCodeError", "ErrorHandler", new { exception = e.ToString() });
            }
        }
    }
}