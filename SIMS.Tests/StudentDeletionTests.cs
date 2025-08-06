using Moq;
using Xunit;
using Microsoft.AspNetCore.Identity;
using SIMS.Web.Controllers;
using SIMS.Web.Data;
using SIMS.Web.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class StudentDeletionTests
{
    [Fact]
    public async Task DeleteConfirmed_DeletesStudentAndUser_WhenStudentExists()
    {
        var context = TestDbContext.GetTestDbContext();
        var mockUserStore = new Mock<IUserStore<ApplicationUser>>();
        var mockUserManager = new Mock<UserManager<ApplicationUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);

        var user = new ApplicationUser { Id = "student-user-1", UserName = "student1@test.com" };
        var student = new Student { Id = 1, ApplicationUserId = "student-user-1" };

        context.Users.Add(user);
        context.Students.Add(student);
        await context.SaveChangesAsync();

        mockUserManager.Setup(um => um.FindByIdAsync("student-user-1")).ReturnsAsync(user);
        mockUserManager.Setup(um => um.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);

        var controller = new StudentsController(context, mockUserManager.Object);

        var result = await controller.DeleteConfirmed(1);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Empty(context.Students.ToList());
        mockUserManager.Verify(um => um.DeleteAsync(user), Times.Once);
    }

    [Fact]
    public async Task DeleteConfirmed_DoesNothing_WhenStudentDoesNotExist()
    {
        var context = TestDbContext.GetTestDbContext();
        var mockUserStore = new Mock<IUserStore<ApplicationUser>>();
        var mockUserManager = new Mock<UserManager<ApplicationUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);

        var controller = new StudentsController(context, mockUserManager.Object);

        var result = await controller.DeleteConfirmed(99);

        Assert.IsType<RedirectToActionResult>(result);
        mockUserManager.Verify(um => um.DeleteAsync(It.IsAny<ApplicationUser>()), Times.Never);
    }
}