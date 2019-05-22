
using System.ComponentModel.DataAnnotations;

namespace TouristTalk.Models
{
    /// <summary>
    /// Model for Register View
    /// </summary>
    public class RegisterViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "User Name is required")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "User Name must be between 5 and 50 characters long")]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Telephone Number is required")]
        [StringLength(20, MinimumLength = 10, ErrorMessage = "Telephone must be between 10 and 20 characters long")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Telephone number must be numeric")]
        [Display(Name = "Telephone No")]
        public string TelNo { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Telephone Number is required")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Password must be between 5 and 50 characters long")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Confirm Password is required")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Password must be between 5 and 50 characters long")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        
        public string ErrorMessage { get; set; }
    }
}