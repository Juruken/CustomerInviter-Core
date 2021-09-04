using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CustomerInvite.Api.Service.Tests.HttpHelpers
{
    public class ClientWrapper
    {
        private readonly HttpClient _client;

        public ClientWrapper(HttpClient client)
        {
            _client = client;
        }

        public async Task<ResponseWrapper> Put(string url, Action<Context> context)
        {
            var ctx = new Context();
            context(ctx);
            return new ResponseWrapper(await _client.PutAsync(url, ctx.GetContent()));
        }

        public async Task<ResponseWrapper> Post(string url, Action<Context> context)
        {
            var ctx = new Context();
            context(ctx);
            return new ResponseWrapper(await _client.PostAsync(url, ctx.GetContent()));
        }

        public async Task<ResponseWrapper> Get(string url, Action<Context> context = null)
        {
            var ctx = new Context();
            context?.Invoke(ctx);
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            foreach (var pair in ctx.Headers)
                request.Headers.Add(pair.Key, pair.Value);
            return new ResponseWrapper(await _client.SendAsync(request));
        }

        public async Task<ResponseWrapper> Delete(string url, Action<Context> context = null)
        {
            var ctx = new Context();
            context?.Invoke(ctx);
            using var request = new HttpRequestMessage(HttpMethod.Delete, url);
            foreach (var pair in ctx.Headers)
                request.Headers.Add(pair.Key, pair.Value);
            request.Content = ctx.GetContent();
            return new ResponseWrapper(await _client.SendAsync(request));
        }
    }
}