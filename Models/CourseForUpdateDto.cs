using System.ComponentModel.DataAnnotations;

namespace CourseLibrary.API.Models
{
    public class CourseForUpdateDto : CourseForManipulateDto
    {
        [Required]
        public override string Description { get => base.Description; set => base.Description = value; }
    }
}
