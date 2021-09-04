using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomerInviter.Api.Service.Services;
using CustomerInviter.Core.Data;
using CustomerInviter.Core.Models;
using CustomerInviter.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CustomerInviter.Api.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ICustomerService _customerService;
        private readonly IGetCustomersQuery _getCustomersQuery;
        private readonly ICreateCustomerAction _createCustomersAction;
        private readonly ICustomerFileIngester _fileIngester;

        public CustomerController(
            IConfiguration configuration, 
            IGetCustomersQuery getCustomersQuery, 
            ICustomerService customerService, 
            ICreateCustomerAction createCustomersAction, 
            ICustomerFileIngester fileIngester
            )
        {
            _configuration = configuration;
            _getCustomersQuery = getCustomersQuery;
            _customerService = customerService;
            _createCustomersAction = createCustomersAction;
            _fileIngester = fileIngester;
        }

        [HttpPut("")]
        public dynamic AddCustomers(IEnumerable<CustomerModel> models)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (models == null || models.ToList().Count == 0)
                return BadRequest("Must send at least one customer");
            
            models.Select(cm => new Customer
            {
                Id = cm.User_Id,
                Name = cm.Name,
                Location = new Coordinates(double.Parse(cm.Latitude), double.Parse(cm.Longitude))
            })
            .ToList()
            .ForEach(_createCustomersAction.Execute);

            return Accepted("Customers added");
        }

        [HttpGet("distance/{distance}")]
        public dynamic GetCustomersByDistance(int distance)
        {
            var headQuarters = new Coordinates(double.Parse(_configuration["Latitude"]), double.Parse(_configuration["Longitude"]));
            var customersByDistance = _customerService.GetCustomersByDistanceInKm(headQuarters, distance).ToList();

            customersByDistance.Sort((a, b) => a.Id > b.Id ? 1 : -1);

            return Ok(customersByDistance.Select(c => new CustomerModel
            {
                User_Id = c.Id,
                Latitude = c.Location.Latitude.ToString(),
                Longitude = c.Location.Longitude.ToString(),
                Name = c.Name
            }));
        }

        [HttpGet]
        public dynamic GetAllCustomers()
        {
            var customers = _getCustomersQuery.Execute();
            return Ok(customers.Select(c => new CustomerModel
            {
                User_Id = c.Id,
                Latitude = c.Location.Latitude.ToString(),
                Longitude = c.Location.Longitude.ToString(),
                Name = c.Name
            }));
        }

        [HttpPut("import")]
        public async Task<dynamic> ImportCustomers()
        {
            if (!Request.Form.Files.Any()) return BadRequest("No files received");

            if (Request.Form.Files.Any(f => f.ContentType != "application/text")) return BadRequest("Accepted file types are: application/text");

            var customers = (await _fileIngester.Ingest(Request.Form.Files))?.ToList();

            if (customers == null || !customers.Any())
                return BadRequest("Files did not contain any valid customer data");

            customers.ForEach(_createCustomersAction.Execute);

            return Accepted("File accepted");
        }
    }
}