using System;
using System.Threading.Tasks;
using IQOptionClient.Http;
using IQOptionClient.Http.Resources;
using IQOptionClient.Http.Resources.V1;
using IQOptionClient.Http.Resources.V1.Models;
using IQOptionClient.Http.ResthSharpHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace IQOptionClient.IntegrationTests
{
    [TestClass]
    public class LoginTests
    {
        private ILogin<LoginResult> _loginOperation;

        [TestInitialize]
        public void Setup()
        {
            var httpConfiguration = Substitute.For<IHttpConfiguration>();
            httpConfiguration.BaseUrl.Returns(new Uri("https://auth.iqoption.com/api/"));
            var restClient = new ReliableRestClientRestSharPolly(httpConfiguration);

            _loginOperation = new LoginRestSharp(restClient);
        }

        [TestMethod]
        public async Task Login()
        {
            //Arrange
            // TODO : remove credentials
            const string username = "wechallp1p3@gmail.com";
            const string password = "t3st!ng";

            //Act
            var response = await _loginOperation.Login(username, password);

            //Assert
            Assert.IsNotNull(response);
            Assert.IsFalse(string.IsNullOrEmpty(response.Ssid));
        }
    }
}
