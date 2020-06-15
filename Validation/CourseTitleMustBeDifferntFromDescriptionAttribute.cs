using CourseLibrary.API.Models;

using System.ComponentModel.DataAnnotations;

namespace CourseLibrary.API.Validation
{
    public class CourseTitleMustBeDifferntFromDescriptionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(
            object value,
            ValidationContext validationContext)
        {
            var course = (CourseForManipulateDto)validationContext.ObjectInstance;
            if (course.Title == course.Description)
            {
                return new ValidationResult(
                    ErrorMessage,
                    new[] { nameof(CourseForManipulateDto) });
            }

            return ValidationResult.Success;
        }
    }
}
