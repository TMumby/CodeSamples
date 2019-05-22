using System;
using System.Web.Mvc;
using TouristTalk.Models;

namespace TouristChat.Controllers
{
    /// <summary>
    /// Controller for user sign on and maintenance
    /// </summary>
    public class AccountController : Controller
    {
        /// <summary>
        /// Basic Login screen
        /// </summary>
        /// <returns>Login screen</returns>
        public ActionResult Login()
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

        /// <summary>
        /// To recieve Login form post, Authorises user
        /// </summary>
        /// <param name="model">LoginView Mod</param>
        /// <returns>If Authorisation successful redirects to Home page, 
        ///          otherwise returns to login with error</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                //Authenticate user
                UserModel userModel = new UserModel(model.UserName, model.Password);

                if (userModel.Authenticated == false)
                {
                    model.ErrorMessage = "Unknown UserName or Incorrect Password";
                    return View(model);
                }
                
                //set session details to include user details, used for Authorisation
                //in other controllers                
                SessionHandler.AddAuthorisedUser(userModel);
                return RedirectToAction("About", "Home");
            }
            catch (Exception e)
            {                
                return RedirectToAction("HandledCodeError", "ErrorHandler", new { exception = e.ToString() });
            }
        }
             
        /// <summary>
        /// Removes all session information, removing user authorisation
        /// </summary>
        /// <returns>Redirects to Home page</returns>
        public ActionResult LogOff()
        {
            try
            {
                SessionHandler.Logoff();
                return RedirectToAction("About", "Home");
            }
            catch (Exception e)
            {
                return RedirectToAction("HandledCodeError", "ErrorHandler", new { exception = e.ToString() });
            }
        }

        /// <summary>
        /// Basic Register user form
        /// </summary>
        /// <returns>Register User view</returns>
        public ActionResult Register()
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

        /// <summary>
        /// Recieves post from Register view. Adds user if not already exists.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Returns Authorised user to Home screen</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                //Add User and check not already exist
                RegisterUserModel registration = new RegisterUserModel(model.UserName, model.Password, model.Email, model.TelNo);

                if (registration.Success)
                {
                    SessionHandler.AddAuthorisedUser(new UserModel(model.UserName, model.Password));                    
                    return RedirectToAction("About", "Home");
                }

                model.ErrorMessage = "Unable to add user, try different credentials";
                return View(model);
            }
            catch (Exception e)
            {
                return RedirectToAction("HandledCodeError", "ErrorHandler", new { exception = e.ToString() });
            }           
        }
    }
}