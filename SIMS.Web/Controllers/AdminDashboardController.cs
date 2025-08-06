using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIMS.Web.Data;
using SIMS.Web.Models;
using System.Threading.Tasks;

namespace SIMS.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalStudents = await _context.Students.CountAsync();
            ViewBag.TotalFaculty = await _context.Faculty.CountAsync();
            ViewBag.TotalCourses = await _context.Courses.CountAsync();

            return View();
        }

        public async Task<IActionResult> GetFacultyList()
        {
            var facultyList = await _context.Faculty.Include(f => f.User).ToListAsync();
            return PartialView("~/Views/Shared/_FacultyListPartial.cshtml", facultyList);
        }

        public async Task<IActionResult> GetCourseList()
        {
            var courseList = await _context.Courses.Include(c => c.Instructor).ToListAsync();
            return PartialView("~/Views/Shared/_CourseListPartial.cshtml", courseList);
        }
        public async Task<IActionResult> GetStudentList()
        {
            var studentList = await _context.Students.ToListAsync();
            return PartialView("~/Views/Shared/_StudentListPartial.cshtml", studentList);
        }
    }
}