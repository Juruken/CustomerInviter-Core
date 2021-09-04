using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace CustomerInvite.Api.Service.Tests.HttpHelpers
{
    public class ResponseWrapper
    {
        private readonly HttpResponseMessage _response;

        public ResponseWrapper(HttpResponseMessage response)
        {
            _response = response;
            Headers = new Dictionary<string, string>(response.Headers.Select(header =>
                new KeyValuePair<string, string>(header.Key, header.Value.First())));
        }

        public HttpStatusCode StatusCode => _response.StatusCode;

        public Dictionary<string, string> Headers { get; }

        public T DeserializeJson<T>()
        {
            return JsonConvert.DeserializeObject<T>(_response.Content.ReadAsStringAsync().Result);
        }
    }
}