using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using CustomerInvite.Api.Service.Tests.HttpHelpers;
using CustomerInviter.Core.Models;
using Shouldly;
using TestStack.BDDfy;
using TestStack.BDDfy.Xunit;
using Xunit.Abstractions;

namespace CustomerInvite.Api.Service.Tests.Scenarios
{

    public class CreateCustomersScenario : ApiScenario
    {
        private ResponseWrapper _response;
        private List<dynamic> _model;
        
        public CreateCustomersScenario(ITestOutputHelper output) : base(output)
        {
            
        }

        public void GivenACustomerToCreate()
        {
            _model = new List<dynamic>
            {
                new
                {
                    Latitude = "52.986375", 
                    user_id = 12, 
                    Name  = "Christina McArdle", 
                    Longitude = "-6.043701" 
                },
                new CustomerModel
                {
                    User_Id = 4, 
                    Name = "Ian Kehoe", 
                    Latitude = "53.2451022", 
                    Longitude = "-6.238335" 
                }
            };
        }

        public async Task WhenCreatingTheCustomer()
        {
            _response = await Client.Put("/customer", with => with.JsonBody(_model));
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
