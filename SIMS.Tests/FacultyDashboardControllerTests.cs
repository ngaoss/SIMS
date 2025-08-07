using Moq;
using Xunit;
using Microsoft.AspNetCore.Identity;
using SIMS.Web.Controllers;
using SIMS.Web.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class FacultyDashboardControllerTests
{
    [Fact]
    public async Task Index_ReturnsViewResult_WithListOfCourses()
    {
        var mockUserStore = new Mock<IUserStore<ApplicationUser>>();
        var mockUserManager = new Mock<UserManager<ApplicationUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);

        var user = new ApplicationUser { Id = "test-user-id", UserName = "faculty@test.com" };
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, user.UserName),
        }));

        mockUserManager.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

        var context = TestDbContext.GetTestDbContext();
        var faculty = new Faculty { Id = 1, ApplicationUserId = "test-user-id" };
        var course = new Course { Id = 101, FacultyId = 1, CourseName = "Test Course" };
        context.Faculty.Add(faculty);
        context.Courses.Add(course);
        await context.SaveChangesAsync();

        var controller = new FacultyDashboardController(context, mockUserManager.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        var result = await controller.Index();


        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Course>>(viewResult.ViewData.Model);
        Assert.Single(model);
    }
       [Fact]
    public async Task CourseDetails_ReturnsViewWithCourseAndStudents_WhenIdIsValid()
    {
        
        var context = TestDbContext.GetTestDbContext();
        var student1 = new Student { Id = 1, FullName = "Alice" };
        var course1 = new Course { Id = 101, CourseName = "Computer Science 101" };
        var enrollment1 = new Enrollment { Id = 1, StudentId = 1, Student = student1, CourseId = 101, Course = course1 };
        
        context.Students.Add(student1);
        context.Courses.Add(course1);
        context.Enrollments.Add(enrollment1);
        await context.SaveChangesAsync();

        var mockUserStore = new Mock<IUserStore<ApplicationUser>>();
        var mockUserManager = new Mock<UserManager<ApplicationUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);
        
        var controller = new FacultyDashboardController(context, mockUserManager.Object);

        var result = await controller.CourseDetails(101); 

        
        var viewResult = Assert.IsType<ViewResult>(result);
        
        var model = Assert.IsAssignableFrom<Course>(viewResult.ViewData.Model);
        
        Assert.Equal("Computer Science 101", model.CourseName);
        
        Assert.NotNull(model.Enrollments);
        Assert.Single(model.Enrollments);
        Assert.Equal("Alice", model.Enrollments.First().Student.FullName);
    }
}
