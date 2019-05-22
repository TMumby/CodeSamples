using System;
using System.Web.Mvc;
using TouristTalk.Models;
using TouristTalk.Validation;

namespace TouristTalk.Controllers
{
    /// <summary>
    /// Controller for returning media files
    /// </summary>
    public class MediaFileController : Controller
    {
        /// <summary>
        /// Returns media file. Note throws an exception if file not for user
        /// </summary>
        /// <param name="fileID"><ID of file to be returned/param>
        /// <returns>media file</returns>
        [AuthorizeUser]
        public ActionResult GetMediaFile(int fileID)
        {
            try
            {
                MessageFileModel file = new MessageFileModel(fileID, SessionHandler.UserDetails.UserID);
                return File(file.Data, file.FileType);
            }
            catch (Exception e)
            {
                return RedirectToAction("HandledCodeError", "ErrorHandler", new { exception = e.ToString() });
            }
        }
    }
}