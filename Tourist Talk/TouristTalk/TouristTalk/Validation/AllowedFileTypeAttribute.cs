using System.Collections.Generic;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TouristTalk.Validation
{
    /// <summary>
    /// Custom Attribute for checking allowed file type
    /// </summary>
    public class AllowedFileTypeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }
            HttpPostedFileBase file = (HttpPostedFileBase)value;
            if (IsValidType(file.ContentType))
            {
                return ValidationResult.Success;
            }
            return new ValidationResult("Not a Valid Type");
            
        }

        protected bool IsValidType(string fileType)
        {
            List<string> fileTypeList = FileTypeList();
            if (fileTypeList.Contains(fileType))
            {
                return true;
            }
            return false;
        }

        protected List<string> FileTypeList()
        {
            List<string> fileTypeList = new List<string>{ "video/mp4", "image/jpeg", "audio/mp3",
                                                          "audio/mpeg"};
            return fileTypeList;
        }



    }
}