using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Demo1.Tests
{
    public class DemoWebApplicationFactory : WebApplicationFactory<Startup> 
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("AutomatedTesting");

            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"ConnectionStrings:DatabaseReadWrite", "Data Source=localhost;Initial Catalog=demo.tests;Integrated Security=true"},
                    {"AnotherConfigKey", "1234"},
                });
            });
        }

        protected override void ConfigureClient(HttpClient client)
        {
            base.ConfigureClient(client);

            client.DefaultRequestHeaders.Add(
                "X-Demo-Header", "Example1234"
                );
        }
        
    }
}


