using MessageApi.Models;
using FluentAssertions;
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
            var returnedLogin = await Client.PostAsync<AccountLogin, ResponseBody<LoginResponse>>("account/login", accountLogin);

            Client!.AddHeader("Authorization", $"Bearer {returnedLogin!.Data!.Token}");
            var response = await Client.GetAsync<ResponseBody<AccountRetrieve>>("account");

            // Assert
            Assert.Multiple(() =>
            {
                response.Title.Should().Be("Success");
                response!.Status.Should().Be(200);
                response.Data!.Username.Should().Be(accountLogin.Username);
                response.Data.Name.Should().Be(returnedLogin.Data.Name);
                response.Data.Status.Should().Be(returnedLogin.Data.Status);
            });
        }

        [Test]
        public async Task GetAccount_GivenValidUsername_ShouldReturnValidAccount()
        {
            // Arrange
            var username = "test-user";
            var accountLogin = new AccountLogin { Username = "toshiba", Password = "Solutions" };

            // Act
            var returnedLogin = await Client.PostAsync<AccountLogin, ResponseBody<LoginResponse>>("account/login", accountLogin);
            Client!.AddHeader("Authorization", $"Bearer {returnedLogin!.Data!.Token}");
            var response = await Client.GetAsync<ResponseBody<AccountRetrieve>>($"account?username={username}");

            // Assert
            Assert.Multiple(() =>
            {
                response!.Status.Should().Be(200);
                response.Data!.Username.Should().Be(username);
            });
        }

        [Test]
        public async Task GetAccount_GivenInvalidUsername_ShouldReturnNotFound()
        {
            // Arrange
            var username = "invaliduser";
            var accountLogin = new AccountLogin { Username = "toshiba", Password = "Solutions" };

            // Act
            var returnedLogin = await Client.PostAsync<AccountLogin, ResponseBody<LoginResponse>>("account/login", accountLogin);
            Client!.AddHeader("Authorization", $"Bearer {returnedLogin!.Data!.Token}");
            var response = await Client.GetAsync<ResponseBody<AccountRetrieve>>($"account?username={username}");

            // Assert
            Assert.Multiple(() =>
            {
                response!.Title.Should().Be("Not Found");
                response.Status.Should().Be(404);
                response.Message.Should().Be($"Account '{username}' was not found");
                response.Data.Should().BeNull();
            });
        }
    }
}