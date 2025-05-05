using MessageApi.Models;
using FluentAssertions;
using System.Text;
using Microsoft.Extensions.Configuration;
using Message.Api.T2.Tests.Tools;

namespace Message.Api.T2.Tests.AccountTests
{
    internal class CreateAccountTests: TestBase
    {
        [Test]
        public async Task CreateAccount_GivenExistingUser_ShouldReturnBadRequest()
        {
            // Arrange
            var newAccount = new AccountCreate
            {
                Username = "toshiba",
            };

            // Act
            var returnedBody = await Client.PostAsync<AccountCreate, ResponseBody>("account", newAccount);

            // Assert
            Assert.Multiple(() =>
            {
                returnedBody!.Title.Should().Be("Already Exists");
                returnedBody.Status.Should().Be(403);
                returnedBody.Message.Should().Be("This username is already taken by another person");
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
            var returnedBody = await Client.PostAsync<AccountCreate, ResponseBody>("account", newAccount);
            var returnedLogin = await Client.PostAsync<AccountCreate, LoginResponse>("account/login", newAccount);
            
            Client!.AddHeader("Authorization", $"Bearer {returnedLogin!.Token}");
            await Client!.DeleteAsync<ResponseBody>("account");

            // Assert
            Assert.Multiple(() =>
            {
                returnedBody.Status.Should().Be(201);
                returnedLogin!.Username.Should().Be(newAccount.Username);
                newAccount!.Name.Should().Be(returnedLogin.Name);
                returnedLogin.Status.Should().Be("Ready to chat");
            });
        }
    }
}
