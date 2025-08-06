using System.ComponentModel.DataAnnotations.Schema;

namespace SIMS.Web.Models
{
    public class Enrollment
    {
        public int Id { get; set; }

        public int StudentId { get; set; } 

        public int CourseId { get; set; }

        public string Semester { get; set; } = string.Empty;
        public float? Grade { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; } = null!;

        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; } = null!;
    }
}