using Azure;
using MessageApi.Models;
using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Message.Api.T2.Tests.Tools
{
    public class ApiClient
    {
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly HttpClient? _client;

        public ApiClient() {
             _jsonOptions = new()
             {
                 PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                 PropertyNameCaseInsensitive = true
             };

            var messageApiUrl = $"{TestContext.Parameters["messageApiUrl"]}/messageApi/";

            _client = new HttpClient
            {
                BaseAddress = new Uri(messageApiUrl)
            };
        }

        public async Task<TResponse> PostAsync<TBody, TResponse>(string url, TBody body)
        {
            var jsonContent = new StringContent(
               JsonSerializer.Serialize(body),
               Encoding.UTF8,
               "application/json"
           );

            var loginResponse = await _client!.PostAsync(url, jsonContent);
            var responseString = await loginResponse.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<TResponse>(responseString, _jsonOptions);
        }

        public async Task<TResponse> GetAsync<TResponse>(string url)
        {
            var response = await _client!.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<TResponse>(responseString, _jsonOptions);
        }

        public async Task<TResponse> DeleteAsync<TResponse>(string url)
        {
            var response = await _client!.DeleteAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<TResponse>(responseString, _jsonOptions);
        }

        public void AddHeader(string name, string value)
        {
            _client!.DefaultRequestHeaders.Add(name, value);
        }
        
        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
