using Xunit;
using Moq;
using SIMS.Web.Controllers;
using SIMS.Web.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SIMS.Web.Data; 
public class EnrollmentControllerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
    private readonly ApplicationDbContext _context;
    private readonly EnrollmentController _controller;
    private readonly ApplicationUser _user;
    private readonly Student _student;

    public EnrollmentControllerTests()
    {
        _context = TestDbContext.GetTestDbContext();
        var mockUserStore = new Mock<IUserStore<ApplicationUser>>();
        _mockUserManager = new Mock<UserManager<ApplicationUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);

        _user = new ApplicationUser { Id = "student-user-id", UserName = "student@test.com" };
        _student = new Student { Id = 1, ApplicationUserId = "student-user-id" };
        
        _context.Users.Add(_user);
        _context.Students.Add(_student);
        _context.SaveChanges();

        _mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(_user);

        _controller = new EnrollmentController(_context, _mockUserManager.Object);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) }
        };
    }

    [Fact]
    public async Task Enroll_CreatesEnrollment_WhenCourseIsValidAndNotEnrolled()
    {
        var course = new Course { Id = 101, CourseName = "New Course" };
        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        var result = await _controller.Enroll(101);

        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectToActionResult.ActionName);
        Assert.Single(_context.Enrollments.ToList()); 
        Assert.Equal("Successfully enrolled in New Course!", _controller.TempData["SuccessMessage"]);
    }

    [Fact]
    public async Task Enroll_ReturnsRedirect_WithErrorMessage_WhenAlreadyEnrolled()
    {
        var course = new Course { Id = 101, CourseName = "Existing Course" };
        var existingEnrollment = new Enrollment { StudentId = _student.Id, CourseId = 101 };
        _context.Courses.Add(course);
        _context.Enrollments.Add(existingEnrollment);
        await _context.SaveChangesAsync();

        var result = await _controller.Enroll(101);

        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectToActionResult.ActionName);
        Assert.Single(_context.Enrollments.ToList()); 
        Assert.Equal("You are already enrolled in this course.", _controller.TempData["ErrorMessage"]);
    }
}