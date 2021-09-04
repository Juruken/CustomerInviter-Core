using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CustomerInvite.Api.Service.Tests.HttpHelpers
{
    public class Context
    {
        private HttpContent _content;
        private readonly Dictionary<string, string> _headers = new Dictionary<string, string>();

        public void Header(string header, string value)
        {
            _headers.Add(header, value);
        }

        public void JsonBody(object body)
        {
            var content = new StringContent(JsonConvert.SerializeObject(body, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            _content = content;
        }

        public HttpContent GetContent()
        {
            if (_content == null) _content = new StringContent("");
            foreach (var pair in _headers)
                _content.Headers.Add(pair.Key, pair.Value);
            return _content;
        }

        public void File(string key, string fileName, string contentType, Stream stream)
        {
            var multipart = new MultipartFormDataContent();
            var streamContent = new StreamContent(stream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            multipart.Add(streamContent, key, fileName);
            _content = multipart;
        }

        public Dictionary<string, string> Headers => _headers;
    }
}