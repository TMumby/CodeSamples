using System.Web;

namespace TouristTalk.Models
{
    public static class SessionHandler
    {

        private static UserModel _userDetails;

        /// <summary>
        /// For checking user is authorised against session data
        /// </summary>
        public static bool Authenticated
        {
            get
            {   
                _userDetails = (UserModel)HttpContext.Current.Session["UserDetails"];
                if (_userDetails != null && _userDetails.Authenticated == true)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// gets Userdetails from the session
        /// </summary>
        public static UserModel UserDetails
        {
            get
            {
                _userDetails = (UserModel)HttpContext.Current.Session["UserDetails"];
                return _userDetails;
            }
        }

        /// <summary>
        /// Sets session data to null. Removes Authorisation
        /// </summary>
        public static void AddAuthorisedUser(UserModel authorisedUser)
        {
            HttpContext.Current.Session["UserDetails"] = authorisedUser;
        }

        /// <summary>
        /// Sets session data to null. Removes Authorisation
        /// </summary>
        public static void Logoff()
        {
            _userDetails = null;
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();
            HttpContext.Current.User = null;
        }


    }
}
