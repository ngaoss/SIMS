using System.ComponentModel.DataAnnotations.Schema; 
using SIMS.Web.Models; 
using System.ComponentModel.DataAnnotations; 

namespace SIMS.Web.Models
{
    public class Course
    {
        [Key] 
        public int Id { get; set; } 

        [Required]
        public string CourseCode { get; set; } = string.Empty;

        [Required]
        public string CourseName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public int Credits { get; set; }
        
        public int? FacultyId { get; set; }
        [ForeignKey("FacultyId")]
        public virtual Faculty? Instructor { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}