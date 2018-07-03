using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace ResultFramework.UnitTests
{
    public class AttemptTests
    {
        private readonly HttpClient _httpClient;

        public AttemptTests()
        {
            this._httpClient = new HttpClient();
            this._httpClient.BaseAddress = new Uri("https://httpbin.org");
        }

        ~AttemptTests()
        {
            this._httpClient.Dispose();
        }

        [Fact]
        public async Task Test1()
        {
            async Task<IResult<string>> testMethod(string endpoint)
            {
                try
                {
                    var response = await this._httpClient.GetAsync("/get");

                    try
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            return new SuccessResult<string>(await response.Content.ReadAsStringAsync());
                        }
                        else
                        {
                            return new ErrorResult<string, HttpStatusCode>(response.StatusCode);
                        }
                    }
                    catch (Exception ex)
                    {
                        return new ErrorResult<string, Exception>(ex, previousResult: new ErrorResult<string, HttpStatusCode>(response.StatusCode));
                    }
                }
                catch (Exception ex)
                {
                    return new ErrorResult<string, Exception>(ex);
                }
            }

            var result = await testMethod("/get");

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.True(result.Value.Length > 0);
        }

        [Fact]
        public async Task Test2()
        {
            async Task<IResult<string>> testMethod(string endpoint)
            {
                // always wrap it so we can capture the exception
                try
                {
                    // perform the action
                    var response = await this._httpClient.GetAsync("/status/400");

                    // assert failure or success
                    if (response.IsSuccessStatusCode)
                    {
                        return new SuccessResult<string>(await response.Content.ReadAsStringAsync());
                    }
                    else
                    {
                        return new ErrorResult<string, HttpStatusCode>(response.StatusCode);
                    }
                }
                catch (Exception ex)
                {
                    return new ErrorResult<string, Exception>(ex);
                }
            }

            var result = await testMethod("/get");

            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.True(result is IErrorResult);
        }

        [Fact]
        public async Task Test3()
        {
            async Task<IResult<string>> testMethod(string endpoint)
            {
                // always wrap it so we can capture the exception
                try
                {
                    // perform the action
                    async Task<HttpResponseMessage> method()
                    {
                        return await this._httpClient.GetAsync("/status/400");
                    }

                    // assert failure or success
                    async Task<IResult<string>> assess(HttpResponseMessage response)
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            return new SuccessResult<string>(await response.Content.ReadAsStringAsync());
                        }
                        else
                        {
                            return new ErrorResult<string, HttpStatusCode>(response.StatusCode);
                        }
                    }

                    return await assess(await method());
                }
                catch (Exception ex)
                {
                    return new ErrorResult<string, Exception>(ex);
                }
            }

            var result = await testMethod("/get");

            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.True(result is IErrorResult);
        }

        [Fact]
        public async Task Test4()
        {
            var result = await Attempt
                .Run(() => this._httpClient.GetAsync("/status/400"))
                .ErrorWhen(r => Task.FromResult(r.IsSuccessStatusCode),
                    async r => await r.Content.ReadAsStringAsync(),
                    r => Task.FromResult(r.StatusCode));

            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.True(result is IErrorResult);
        }
    }
}
