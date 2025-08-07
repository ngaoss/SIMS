using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIMS.Web.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string StudentCode { get; set; } = string.Empty; 

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty; 

        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; } = string.Empty; 
        
        public string ApplicationUserId { get; set; } = string.Empty; 

        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser User { get; set; } = null!; 

        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>(); 
    }
}