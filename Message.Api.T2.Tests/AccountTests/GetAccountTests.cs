using MessageApi.Models;
using FluentAssertions;
using System.Text;
using Microsoft.Extensions.Configuration;
using Message.Api.T2.Tests.Tools;

namespace Message.Api.T2.Tests.AccountTests
{
   internal class GetAccoutTests: TestBase
   {

        [Test]
        public async Task GetAccount_AfterLogin_ShouldReturnAuthenticatedAccount()
        {
            // Arrange
            var accountLogin = new AccountLogin
            {
                Username = "toshiba",
                Password = "Solutions"
            };

            // Act
            var returnedLogin = await Client.PostAsync<AccountLogin, LoginResponse>("account/login", accountLogin);

            Client!.AddHeader("Authorization", $"Bearer {returnedLogin!.Token}");
            var returnedAccount = await Client.GetAsync<AccountRetrieve>("account");

            // Assert
            Assert.Multiple(() =>
            {
                returnedAccount!.Username.Should().Be(accountLogin.Username);
                returnedAccount!.Name.Should().Be(returnedLogin.Name);
                returnedAccount!.Status.Should().Be(returnedLogin.Status);
            });
        }

        [Test]
        public async Task GetAccount_GivenValidUsername_ShouldReturnValidAccount()
        {
            // Arrange
            var username = "toshiba";
            var jwtSettings = AppConfig.GetSection("JwtSettings");
            var token = jwtSettings["Token"];

            // Act

            Client!.AddHeader("Authorization", $"Bearer {token}");
            var returnedAccount = await Client.GetAsync<AccountRetrieve>($"account?username={ username }");

            // Assert
            Assert.Multiple(() =>
            {
                returnedAccount!.Username.Should().Be(username);
            });
        }

        [Test]
        public async Task GetAccount_GivenInvalidUsername_ShouldReturnNotFound()
        {
            // Arrange
            var username = "invaliduser";
            var jwtSettings = AppConfig.GetSection("JwtSettings");
            var token = jwtSettings["Token"];

            // Act

            Client!.AddHeader("Authorization", $"Bearer {token}");
            var returnedBody = await Client.GetAsync<ResponseBody>($"account?username={username}");

            // Assert
            Assert.Multiple(() =>
            {
                returnedBody!.Title.Should().Be("Not Found");
                returnedBody.Status.Should().Be(404);
            });
        }
    }
}