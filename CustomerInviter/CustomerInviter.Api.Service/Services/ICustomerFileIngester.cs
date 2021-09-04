using System.Collections.Generic;
using System.Threading.Tasks;
using CustomerInviter.Core.Models;
using Microsoft.AspNetCore.Http;

namespace CustomerInviter.Api.Service.Services
{
    public interface ICustomerFileIngester
    {
        Task<IEnumerable<Customer>> Ingest(IFormFileCollection files);
    }
}