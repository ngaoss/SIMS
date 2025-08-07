using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIMS.Web.Data;
using SIMS.Web.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SIMS.Web.Controllers
{
    [Authorize(Roles = "Faculty")]
    public class FacultyDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FacultyDashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return Challenge(); 
            }

            var facultyProfile = await _context.Faculty
                .FirstOrDefaultAsync(f => f.ApplicationUserId == currentUser.Id);

            if (facultyProfile == null)
            {
                return NotFound("Faculty profile not found for the current user.");
            }

            var assignedCourses = await _context.Courses
                .Where(c => c.FacultyId == facultyProfile.Id)
                .ToListAsync();

            return View(assignedCourses);
        }

        public async Task<IActionResult> CourseDetails(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.Student)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateGrade(int enrollmentId, int courseId, float? grade)
        {
            var enrollment = await _context.Enrollments.FindAsync(enrollmentId);

            if (enrollment == null)
            {
                return NotFound();
            }

            enrollment.Grade = grade;
            _context.Update(enrollment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Grade updated successfully!";

            return RedirectToAction(nameof(CourseDetails), new { id = courseId });
        }
        
    } 
}