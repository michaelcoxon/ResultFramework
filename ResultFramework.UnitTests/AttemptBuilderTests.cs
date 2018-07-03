using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace ResultFramework.UnitTests
{
    public class AttemptBuilderTests
    {
        private readonly HttpClient _httpClient;

        public AttemptBuilderTests()
        {
            this._httpClient = new HttpClient();
            this._httpClient.BaseAddress = new Uri("https://httpbin.org");
        }

        ~AttemptBuilderTests()
        {
            this._httpClient.Dispose();
        }

        [Fact]
        public async Task Test4()
        {
            var builder = AttemptBuilder
                .Run(() => this._httpClient.GetAsync("/status/400"))
                .ErrorWhen(r => r.StatusCode == HttpStatusCode.BadRequest, r => Task.FromResult(r.StatusCode))
                .ErrorWhen(r => !r.IsSuccessStatusCode, r => Task.FromResult(r.StatusCode));

            var result = await builder.ExecuteAsync();

            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.True(result is IErrorResult);
        }

        [Fact]
        public async Task Test5()
        {
            var builder = AttemptBuilder
                .Run(() => this._httpClient.GetAsync("/status/400"))
                .ErrorWhen(r => r.StatusCode == HttpStatusCode.NotFound, r => Task.FromResult(r.StatusCode))
                .ErrorWhen(r => !r.IsSuccessStatusCode, r => Task.FromResult(r.StatusCode));

            var result = await builder.ExecuteAsync();

            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.True(result is IErrorResult);
        }
    }
}
