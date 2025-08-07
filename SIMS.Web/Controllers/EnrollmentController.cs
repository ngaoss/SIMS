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
    [Authorize(Roles = "Student")]
    public class EnrollmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EnrollmentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var studentProfile = await _context.Students.FirstOrDefaultAsync(s => s.ApplicationUserId == currentUser.Id);

            if (studentProfile == null)
            {
                return NotFound("Student profile not found.");
            }

            var enrolledCourseIds = await _context.Enrollments
                .Where(e => e.StudentId == studentProfile.Id)
                .Select(e => e.CourseId)
                .ToListAsync();

            var availableCourses = await _context.Courses
                .Where(c => !enrolledCourseIds.Contains(c.Id))
                .ToListAsync();

            return View(availableCourses);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(int courseId)
        {

            var currentUser = await _userManager.GetUserAsync(User);
            var studentProfile = await _context.Students.FirstOrDefaultAsync(s => s.ApplicationUserId == currentUser.Id);

            if (studentProfile == null)
            {
                TempData["ErrorMessage"] = "Could not find your student profile.";
                return RedirectToAction(nameof(Index));
            }

            var courseToEnroll = await _context.Courses.FindAsync(courseId);
            if (courseToEnroll == null)
            {
                TempData["ErrorMessage"] = "The selected course does not exist.";
                return RedirectToAction(nameof(Index));
            }

            bool isAlreadyEnrolled = await _context.Enrollments
                .AnyAsync(e => e.StudentId == studentProfile.Id && e.CourseId == courseId);

            if (isAlreadyEnrolled)
            {
                TempData["ErrorMessage"] = "You are already enrolled in this course.";
                return RedirectToAction(nameof(Index));
            }


            var newEnrollment = new Enrollment
            {
                StudentId = studentProfile.Id,
                CourseId = courseId,
                Semester = "Fall 2025" 
            };

            _context.Enrollments.Add(newEnrollment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Successfully enrolled in {courseToEnroll.CourseName}!";

            return RedirectToAction(nameof(Index));
        }
    }
}