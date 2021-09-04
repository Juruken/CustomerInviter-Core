using System.Net.Http;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using CustomerInvite.Api.Service.Tests.HttpHelpers;
using Serilog;
using Xunit.Abstractions;

namespace CustomerInvite.Api.Service.Tests.Scenarios
{
    public abstract class ApiScenario
    {
        protected ClientWrapper Client;
        protected ITestOutputHelper Output;
        protected ILifetimeScope Scope;

        protected ApiScenario(ITestOutputHelper output)
        {
            Output = output;
        }

        public virtual void Setup()
        {
            ConfigureLogging(Output);

            var testFactory = new TestWebApplicationFactory();
            var autofacServiceProvider = testFactory.Services as AutofacServiceProvider;
            Scope = autofacServiceProvider.LifetimeScope;

            Client = new ClientWrapper(testFactory.CreateClient());
        }

        /// <summary>
        /// Cleanup any testing resources we've created e.g. test databases etc.
        /// </summary>
        public virtual void TearDown()
        {
            Scope.Disposer.Dispose();
        }

        private static void ConfigureLogging(ITestOutputHelper output)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.XunitTestOutput(output)
                .CreateLogger();
        }
    }
}
