using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ContactlessEntry.Cloud.UnitTests.Utilities
{
    public class TestRequestService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TestRequestService> _logger;

        public TestRequestService(
            HttpClient httpClient,
            ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<TestRequestService>();

            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<TResult> GetAsync<TResult>(string uri, string token = "", Func<HttpResponseMessage, bool> handleResponse = null)
        {
            if (string.IsNullOrEmpty(uri))
            {
                throw new ArgumentNullException(nameof(uri));
            }

            _logger.LogDebug("GET '{0}'", uri);

            var httpClient = CreateHttpClient(token);
            var response = await httpClient.GetAsync(uri);

            if (null == handleResponse || !handleResponse(response))
            {
                await HandleResponse(response);
            }

            var serialized = await response.Content.ReadAsStringAsync();
            _logger.LogDebug("Response Data: {0}", serialized);

            return string.IsNullOrWhiteSpace(serialized) ? default : await Task.Run(() => JsonSerializer.Deserialize<TResult>(serialized));
        }

        public Task<TResult> PostAsync<TResult>(string uri, TResult data, string token = "", Func<HttpResponseMessage, bool> handleResponse = null) => PostAsync<TResult, TResult>(uri, data, token, handleResponse);

        public async Task<TResult> PostAsync<TRequest, TResult>(string uri, TRequest data, string token = "", Func<HttpResponseMessage, bool> handleResponse = null)
        {
            if (string.IsNullOrEmpty(uri))
            {
                throw new ArgumentNullException(nameof(uri));
            }

            _logger.LogDebug("POST '{0}'", uri);

            var serialized = await Task.Run(() => JsonSerializer.Serialize(data));
            _logger.LogDebug("Request Data: {0}", serialized);

            var httpClient = CreateHttpClient(token);
            var response = await httpClient.PostAsync(uri, new StringContent(serialized, Encoding.UTF8, "application/json"));

            if (null == handleResponse || !handleResponse(response))
            {
                await HandleResponse(response);
            }

            var responseData = await response.Content.ReadAsStringAsync();
            _logger.LogDebug("Response Data: {0}", responseData);

            return string.IsNullOrWhiteSpace(responseData)
                ? default
                : JsonSerializer.Deserialize<TResult>(responseData, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }

        public async Task<TResult> PostStreamAsync<TResult>(string uri, Stream stream, string token = "", Func<HttpResponseMessage, bool> handleResponse = null)
        {
            if (string.IsNullOrEmpty(uri))
            {
                throw new ArgumentNullException(nameof(uri));
            }

            _logger.LogDebug("POST '{0}'", uri);

            var httpClient = CreateHttpClient(token);
            var content = new StreamContent(stream);
            content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Application.Octet);

            var response = await httpClient.PostAsync(uri, content);
            if (null == handleResponse || !handleResponse(response))
            {
                await HandleResponse(response);
            }

            var responseData = await response.Content.ReadAsStringAsync();
            _logger.LogDebug("Response Data: {0}", responseData);

            return string.IsNullOrWhiteSpace(responseData)
                ? default
                : JsonSerializer.Deserialize<TResult>(responseData, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }

        public Task<TResult> PutAsync<TResult>(string uri, TResult data, string token = "", Func<HttpResponseMessage, bool> handleResponse = null) => PutAsync<TResult, TResult>(uri, data, token, handleResponse);

        public async Task<TResult> PutAsync<TRequest, TResult>(string uri, TRequest data, string token = "", Func<HttpResponseMessage, bool> handleResponse = null)
        {
            if (string.IsNullOrEmpty(uri))
            {
                throw new ArgumentNullException(nameof(uri));
            }

            _logger.LogDebug("PUT '{0}'", uri);

            var serialized = await Task.Run(() => JsonSerializer.Serialize(data));
            _logger.LogDebug("Request Data: {0}", serialized);

            var httpClient = CreateHttpClient(token);
            var response = await httpClient.PutAsync(uri, new StringContent(serialized, Encoding.UTF8, "application/json"));

            if (null == handleResponse || !handleResponse(response))
            {
                await HandleResponse(response);
            }

            var responseData = await response.Content.ReadAsStringAsync();
            _logger.LogDebug("Response Data: {0}", responseData);

            return string.IsNullOrWhiteSpace(responseData)
                ? default
                : JsonSerializer.Deserialize<TResult>(responseData);
        }

        public async Task DeleteAsync(string uri, string token = "", Func<HttpResponseMessage, bool> handleResponse = null)
        {
            if (string.IsNullOrEmpty(uri))
            {
                throw new ArgumentNullException(nameof(uri));
            }

            _logger.LogDebug("DELETE '{0}'", uri);

            var httpClient = CreateHttpClient(token);
            var response = await httpClient.DeleteAsync(uri);

            if (null == handleResponse || !handleResponse(response))
            {
                await HandleResponse(response);
            }
        }

        private HttpClient CreateHttpClient(string token = "")
        {
            if (!string.IsNullOrWhiteSpace(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return _httpClient;
        }

        private async Task HandleResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                if (HttpStatusCode.Forbidden == response.StatusCode || HttpStatusCode.Unauthorized == response.StatusCode)
                {
                    throw new Exception(content);
                }

                throw new HttpRequestException(content);
            }
        }
    }
}
