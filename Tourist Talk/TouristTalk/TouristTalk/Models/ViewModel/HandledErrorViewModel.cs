
namespace TouristTalk.Models
{
    /// <summary>
    /// Used to hold display data items for Handeled Code Error View
    /// </summary>
    public class HandledErrorViewModel
    {
        public string ErrorMessage { get; set; }

        public HandledErrorViewModel(string error)
        {
            ErrorMessage = error;
        }
    }
}