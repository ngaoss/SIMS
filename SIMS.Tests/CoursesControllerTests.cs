using Xunit;
using Moq;
using SIMS.Web.Controllers;
using SIMS.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class CoursesControllerTests
{
    [Fact]
    public async Task Details_ReturnsViewResult_WithCourse_WhenIdIsValid()
    {
        var context = TestDbContext.GetTestDbContext();
        context.Courses.Add(new Course { Id = 1, CourseName = "Math 101" });
        await context.SaveChangesAsync();

        var controller = new CoursesController(context);

        var result = await controller.Details(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<Course>(viewResult.ViewData.Model);
        Assert.Equal(1, model.Id);
    }

    [Fact]
    public async Task Details_ReturnsNotFoundResult_WhenIdIsInvalid()
    {
        var context = TestDbContext.GetTestDbContext();
        var controller = new CoursesController(context);

        var result = await controller.Details(99);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Create_RedirectsToIndex_WhenModelStateIsValid()
    {
        var context = TestDbContext.GetTestDbContext();
        var controller = new CoursesController(context);
        var newCourse = new Course { CourseCode = "CS101", CourseName = "Intro to CS" };

        var result = await controller.Create(newCourse);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Single(context.Courses.ToList());
    }

    [Fact]
    public async Task Create_ReturnsViewResult_WhenModelStateIsInvalid()
    {
        var context = TestDbContext.GetTestDbContext();
        var controller = new CoursesController(context);
        var newCourse = new Course { CourseName = "Invalid Course" };
        controller.ModelState.AddModelError("CourseCode", "Required");

        var result = await controller.Create(newCourse);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsAssignableFrom<Course>(viewResult.ViewData.Model);
        Assert.Empty(context.Courses.ToList());
    }
}