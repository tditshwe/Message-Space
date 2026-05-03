using MessageApi.Models;
using FluentAssertions;

namespace Message.Api.T2.Tests.AccountTests
{
    internal class CreateAccountTests: TestBase
    {
        [Test]
        public async Task CreateAccount_GivenExistingUser_ShouldReturnAlreadyExists()
        {
            // Arrange
            var newAccount = new AccountCreate
            {
                Username = "toshiba",
            };

            // Act
            var response = await Client.PostAsync<AccountCreate, ResponseBody<Account>>("account", newAccount);

            // Assert
            Assert.Multiple(() =>
            {
                response!.Title.Should().Be("Already Exists");
                response.Status.Should().Be(403);
                response.Message.Should().Be("This username is already taken by another person");
                response.Data.Should().BeNull();
            });
        }

        [Test]
        public async Task CreateAccount_GivenValidUser_ShouldCreateUserAndReturnSuccess()
        {
            // Arrange
            var newAccount = new AccountCreate
            {
                Username = "sinbad",
                Password = "user-pass",
                Name = "Sinbad Tahoma",
            };

            // Act
            var response = await Client.PostAsync<AccountCreate, ResponseBody<Account>>("account", newAccount);
            var returnedLogin = await Client.PostAsync<AccountLogin, ResponseBody<LoginResponse>>("account/login", new AccountLogin
            {
                Username = newAccount.Username,
                Password = newAccount.Password
            });

            Client!.AddHeader("Authorization", $"Bearer {returnedLogin!.Data!.Token}");
            await Client!.DeleteAsync<ResponseBody<int>>("account");

            // Assert
            Assert.Multiple(() =>
            {
                response!.Status.Should().Be(201);
                response.Title.Should().Be("Created");
                response.Data!.Username.Should().Be(newAccount.Username);
                response.Data.Name.Should().Be(newAccount.Name);
                response.Data.Status.Should().Be("Ready to chat");
                returnedLogin.Data!.Username.Should().Be(newAccount.Username);
            });
        }
    }
}
