using Xunit;
using Moq;
using SIMS.Web.Controllers; 
using SIMS.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
public class FacultyControllerTests
{

    [Fact]
    public async Task Create_FailsAndReturnsView_WhenPasswordIsWeak()
    {
        
        var mockUserStore = new Mock<IUserStore<ApplicationUser>>();
        var mockUserManager = new Mock<UserManager<ApplicationUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);

        var weakPassword = "Password123";
        var facultyInfo = new Faculty { FullName = "Test Faculty", Department = "Test Dept" };
        
        var identityError = new IdentityError { Description = "Passwords must have at least one non-alphanumeric character." };
        mockUserManager
            .Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), weakPassword))
            .ReturnsAsync(IdentityResult.Failed(identityError));

        var context = TestDbContext.GetTestDbContext();

        var controller = new FacultyController(context, mockUserManager.Object);


        var username = $"faculty.{facultyInfo.FullName.ToLower().Replace(" ", ".")}";
        var email = $"{username}@university.edu";
        var user = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true };
        
        var result = await mockUserManager.Object.CreateAsync(user, weakPassword);
        
        if (!result.Succeeded)
        {
            foreach(var error in result.Errors)
            {
                controller.ModelState.AddModelError(string.Empty, error.Description);
            }
        }


        
        Assert.False(result.Succeeded);
        
        Assert.False(controller.ModelState.IsValid);
        
        Assert.Contains(controller.ModelState.Values.SelectMany(v => v.Errors), 
                        e => e.ErrorMessage == "Passwords must have at least one non-alphanumeric character.");
    }
}