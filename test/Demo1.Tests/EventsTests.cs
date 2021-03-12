using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;

namespace Demo1.Tests
{
    public class EventsTests
    {
        private record Event(Guid Id, string Name, DateTimeOffset Start);
        
        [Fact]
        public async Task GettingAllEvents_Returns200OK()
        {
            using var server = new WebApplicationFactory<Startup>();

            using var httpClient = server.CreateClient();

            var httpResponseMessage = await httpClient.GetAsync("/events");

            httpResponseMessage.StatusCode.Should().Be(StatusCodes.Status200OK);
        }
        
        [Fact]
        public async Task PostingEvent_CreatesNewEventResource()
        {
            using var server = new WebApplicationFactory<Startup>();
            
            using var httpClient = server.CreateClient();

        }

        private static StringContent CreateEventStringContent(Event expectedEvent)
        {
            var json = $@"{{
                ""Id"": ""{expectedEvent.Id}"",
                ""name"": ""{expectedEvent.Name}"",
                ""Start"": ""{expectedEvent.Start:O}""
            }}";
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return content;
        }

        [Fact]
        public async Task DeletingUnknownEvent_Returns404_WhenAuthorized()
        {
            using var factory = new DemoWebApplicationFactory()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services =>
                    {
                        services.AddAuthentication("Test")
                            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                                "Test", options => { });
                    });
                });

            using var httpClient = factory.CreateClient();

            var responseMessage = await httpClient.DeleteAsync($"/events/{Guid.NewGuid()}");

            responseMessage.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }
        
    }
    
    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, 
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "Test user"),
                new Claim(ClaimTypes.Role, "Administrators")
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Test");

            var result = AuthenticateResult.Success(ticket);

            return Task.FromResult(result);
        }
    }
}
