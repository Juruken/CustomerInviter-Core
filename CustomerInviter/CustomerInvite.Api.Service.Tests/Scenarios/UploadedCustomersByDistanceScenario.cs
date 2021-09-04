using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using CustomerInvite.Api.Service.Tests.HttpHelpers;
using NUnit.Framework;
using Shouldly;
using TestStack.BDDfy;
using TestStack.BDDfy.Xunit;
using Xunit.Abstractions;

namespace CustomerInvite.Api.Service.Tests.Scenarios
{

    public class UploadCustomerFileScenario : ApiScenario
    {
        private ResponseWrapper _response;
        private Stream _fileStream;
        
        public UploadCustomerFileScenario(ITestOutputHelper output) : base(output)
        {
            Output = output;
        }

        public void GivenACustomerFile()
        {
            _fileStream = GetType().GetTypeInfo().Assembly
                .GetManifestResourceStream("CustomerInvite.Api.Service.Tests.customers.txt");
        }

        public async Task WhenUploadingTheCustomerFile()
        {
            _response = await Client.Put("/customer/import", with =>
            {
                with.File("customers", "customers.txt", "application/text",_fileStream);
            });
        }

        public void ThenTheResponseShouldBeAccepted()
        {
            _response.StatusCode.ShouldBe(HttpStatusCode.Accepted);
        }

        [BddfyFact]
        public void Fact()
        {
            this.BDDfy();
        }
    }
}