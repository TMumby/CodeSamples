
using System.ComponentModel.DataAnnotations;

namespace TouristTalk.Models
{
    /// <summary>
    /// Used to hold display data items for Login View
    /// Acts as conduit between Login view and User model.
    /// </summary>
    public class LoginViewModel
    {
        [Required]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "User Name must be between 5 and 50 characters long")]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Password must be between 5 and 50 characters long")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        
        public string ErrorMessage { get; set; }

    }
}