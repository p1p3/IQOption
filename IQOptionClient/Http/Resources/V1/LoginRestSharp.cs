using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;

namespace IQOptionClient.Http.Resources.V1
{
    using LoginResult = Models.LoginResult;

    public class LoginRestSharp : ILogin<LoginResult>
    {
        private readonly IRestClient _restClient;
        private readonly string _resourceName = "v1.0/login";
        
        public LoginRestSharp(IRestClient restClient)
        {

            _restClient = restClient;
        }

        public async Task<LoginResult> Login(string username, string password)
        {
            var request = new RestRequest(_resourceName, Method.POST);
            
            request.AddParameter("email", username);
            request.AddParameter("password", password);

            var response = await _restClient.ExecuteTaskAsync(request);

            var ssid = response.Cookies.FirstOrDefault(cookie =>
                cookie.Name.Equals("ssid", StringComparison.InvariantCultureIgnoreCase)).Value;

            var responseContent = new LoginResult(ssid);

            return responseContent;
        }

    }
}