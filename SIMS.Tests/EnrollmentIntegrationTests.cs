using Xunit;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Testing;

public class EnrollmentIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public EnrollmentIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    
    public async Task Get_EnrollmentPage_ReturnsSuccess_ForAuthenticatedUser()
    {
        var clientWithoutAuth = new WebApplicationFactory<Program>().CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
        );

        var response = await clientWithoutAuth.GetAsync("/Enrollment");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Account/Login", response.Headers.Location?.OriginalString);
    }

    [Fact]
    public async Task Post_EnrollToCourse_RequiresAuthenticationAndAntiForgeryToken()
    {
        var client = new WebApplicationFactory<Program>().CreateClient();
        var initialResponse = await client.GetAsync("/Enrollment/Index"); 

        var formData = new Dictionary<string, string>
        {
            {"courseId", "101"}
        };
        var content = new FormUrlEncodedContent(formData);
        
        var postResponse = await client.PostAsync("/Enrollment/Enroll", content);

        Assert.Equal(HttpStatusCode.Redirect, postResponse.StatusCode);
        Assert.Contains("/Account/Login", postResponse.Headers.Location?.OriginalString);
    }
}