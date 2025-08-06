using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using System.Net;
using System.Net.Http;

public class SecurityIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public SecurityIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    }

    [Theory]
    [InlineData("/")]
    public async Task Get_PublicEndpoints_ReturnSuccess(string url)
    {
        var response = await _client.GetAsync(url);

        response.EnsureSuccessStatusCode(); 
        Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());
    }
    
    [Theory]
    [InlineData("/AdminDashboard")]
    [InlineData("/Students/Create")]
    [InlineData("/Faculty")]
    [InlineData("/Courses")]
    public async Task Get_SecureAdminEndpoints_Redirects_WhenNotAuthenticated(string url)
    {
        var response = await _client.GetAsync(url);

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Account/Login", response.Headers.Location.OriginalString);
    }
    
    [Theory]
    [InlineData("/StudentDashboard")]
    [InlineData("/Enrollment")]
    public async Task Get_SecureStudentEndpoints_Redirects_WhenNotAuthenticated(string url)
    {
        var response = await _client.GetAsync(url);

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Account/Login", response.Headers.Location.OriginalString);
    }
}