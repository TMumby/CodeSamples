using System;
using System.Web.Mvc;
using System.Configuration;
using TouristTalk.Models;

namespace TouristTalk.Controllers
{
    /// <summary>
    /// Controls redirection to error pages
    /// </summary>
    public class ErrorHandlerController : Controller
    {
        

        /// <summary>
        /// customError (web.config) redirects to here. 
        /// </summary>
        /// <returns>Returns view with basic appology, no details</returns>
        public ActionResult Error()
        {
            return View();
        }
        
        /// <summary>
        /// Decides which error page to return depending on web.config displayhandlederrors attribute
        /// </summary>
        /// <param name="exception"></param>
        /// <returns>YSOD - returns "Yellow Screen of death" (customError overrules)
        ///          off - returns appology screen only
        ///          on  - returns exception details but in website layout</returns>
        public ActionResult HandledCodeError(string exception)
        {
            switch (ConfigurationManager.AppSettings["displayhandlederrors"])
            {
                case "YSOD":  //This will depend on setting of customError
                    throw new Exception(exception);                                    
                case "off":
                    return RedirectToAction("Error");
                case "on":                
                    HandledErrorViewModel model = new HandledErrorViewModel(exception);
                    return View(model); 
                default:                
                    throw new Exception("Error handling exception");             
            }        
        }
    }
}