using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace SIMS.Tests
{
    public class AdminDashboardIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public AdminDashboardIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Get_AdminDashboard_Redirects_WhenUserIsNotAuthenticated()
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false 
            });

            var response = await client.GetAsync("/AdminDashboard");

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            
            Assert.NotNull(response.Headers.Location);

            Assert.Contains("/Account/Login", response.Headers.Location.OriginalString);
        }
    }
}