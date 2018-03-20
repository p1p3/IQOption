using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using RestSharp;
using System.Linq;
using System.Net.Http;

namespace IQOptionClient.Http.ResthSharpHelpers
{
    public class ReliableRestClientRestSharPolly : RestClient
    {
        private readonly HttpStatusCode[] _httpStatusCodesWorthRetrying = {
            HttpStatusCode.RequestTimeout, // 408
            HttpStatusCode.InternalServerError, // 500
            HttpStatusCode.BadGateway, // 502
            HttpStatusCode.ServiceUnavailable, // 503
            HttpStatusCode.GatewayTimeout // 504,
            ,HttpStatusCode.Forbidden
        };

        private readonly IAsyncPolicy<IRestResponse> _retryPolicy;
        private readonly int _maxRetryCount = 5;

        public ReliableRestClientRestSharPolly(IHttpConfiguration httpConfiguration) : base(httpConfiguration.BaseUrl)
        {
            _retryPolicy = Policy
                .HandleResult<IRestResponse>(r => _httpStatusCodesWorthRetrying.Contains(r.StatusCode))
                .WaitAndRetryAsync(_maxRetryCount,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                );
        }

        public override Task<IRestResponse> ExecuteTaskAsync(IRestRequest request)
        {
            return this.ExecuteTaskAsync(request, CancellationToken.None);
        }

        public override async Task<IRestResponse> ExecuteTaskAsync(IRestRequest request, CancellationToken token)
        {
            var response = await _retryPolicy.ExecuteAsync(async () => await base.ExecuteTaskAsync(request, token));
            if (!response.IsSuccessful) throw new HttpRequestException($"Response code : {response.StatusCode}, Response contet : {response.Content}");

            return response;
        }
    }
}