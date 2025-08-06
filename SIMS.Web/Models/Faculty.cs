using System.ComponentModel.DataAnnotations.Schema; 
using SIMS.Web.Models; 
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation; // <-- THÊM DÒNG NÀY

namespace SIMS.Web.Models
{
    public class Faculty
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string ApplicationUserId { get; set; } = string.Empty;

        [ForeignKey("ApplicationUserId")]
       [ValidateNever]
        public virtual ApplicationUser User { get; set; } = null!;

        [ValidateNever] 
        public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}