using Autofac;
using CustomerInviter.Api.Service;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace CustomerInvite.Api.Service.Tests
{
    public class TestWebApplicationFactory : WebApplicationFactory<Startup>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureContainer<ContainerBuilder>(builder =>
            {
                // Register any dummy services we need for our tests
                Program.ConfigureAutofacContainer(builder);
            });

            return base.CreateHost(builder);
        }
    }
}