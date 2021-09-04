using System.Collections.Generic;
using System.Linq;
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

    public class GetCustomersByDistanceScenario : ApiScenario
    {
        private ResponseWrapper _response;
        
        public GetCustomersByDistanceScenario(ITestOutputHelper output) : base(output)
        {
        }

        public async Task GivenMultipleCustomers()
        {
            var customers = new List<dynamic>
            {
                new CustomerModel
                {
                    Latitude = "52.986375", 
                    User_Id = 12, 
                    Name  = "Christina McArdle", 
                    Longitude = "-6.043701" 
                },
                new CustomerModel
                {
                    User_Id = 27, 
                    Name = "Enid Gallagher", 
                    Latitude = "54.1225", 
                    Longitude = "-8.143333" 
                },
                new CustomerModel
                {
                    User_Id = 39, 
                    Name = "Lisa Ahearn", 
                    Latitude = "53.0033946", 
                    Longitude = "-6.3877505" 
                }
            };

            await Client.Put("/customer", with =>
            {
                with.JsonBody(customers);
            });
        }

        public async Task WhenFetchingCustomersInDistance()
        {
            _response = await Client.Get($"/customer/distance/{100}");
        }

        public void ThenTheResponseShouldBeOK()
        {
            _response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        public void AndThenTheResponseShouldBeOnlyTwoCustomersInRange()
        {
            var result = _response.DeserializeJson<List<CustomerModel>>();

            result.ShouldNotBeNull();
            result.ShouldNotBeEmpty();
            result.Count.ShouldBe(2);
        }

        public void AndThenTheResultsShouldBeSorted()
        {
            var result = _response.DeserializeJson<List<CustomerModel>>();
            var customer = result.FirstOrDefault();
            customer.Latitude.ShouldBe("52.986375");
            customer.Longitude.ShouldBe("-6.043701");
            customer.Name.ShouldBe("Christina McArdle");
            customer.User_Id.ShouldBe(12);
        }

        [BddfyFact]
        public void Fact()
        {
            this.BDDfy();
        }
    }
}
