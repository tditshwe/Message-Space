using MessageApi.Models;
using FluentAssertions;
using Message.Api.T2.Tests.Tools;

namespace Message.Api.T2.Tests.AccountTests
{
    internal class EditTests : TestBase
    {
        [Test]
        public async Task EditAccount_GivenEmptyBody_ShouldReturnBadRequest()
        {
            // Arrange
            var login = new AccountLogin { Username = "toshiba", Password = "Solutions" };
            var returnedLogin = await Client.PostAsync<AccountLogin, ResponseBody<LoginResponse>>("account/login", login);
            Client.AddHeader("Authorization", $"Bearer {returnedLogin!.Data!.Token}");

            // Act
            var response = await Client.PostAsync<AccountEdit?, ResponseBody<int>>("account", null);

            // Assert
            response.Should().NotBeNull();
            response!.Status.Should().Be(400);
        }

        [Test]
        public async Task EditAccount_GivenValidName_ShouldUpdateAndReturnSuccess()
        {
            // Arrange
            var login = new AccountLogin { Username = "toshiba", Password = "Solutions" };
            var returnedLogin = await Client.PostAsync<AccountLogin, ResponseBody<LoginResponse>>("account/login", login);
            Client.AddHeader("Authorization", $"Bearer {returnedLogin!.Data!.Token}");

            var edit = new AccountEdit { Name = "Toshiba Updated" };

            // Act
            var response = await Client.PutAsync<AccountEdit, ResponseBody<int>>("account", edit);

            // Assert
            response.Should().NotBeNull();
            response!.Status.Should().Be(200);
            response.Title.Should().Be("Updated");

            // Confirm the change persisted
            var account = await Client.GetAsync<ResponseBody<AccountRetrieve>>("account");
            account!.Data!.Name.Should().Be("Toshiba Updated");

            // Restore original name
            await Client.PutAsync<AccountEdit, ResponseBody<int>>("account", new AccountEdit { Name = "Toshiba" });
        }

        [Test]
        public async Task EditAccount_GivenValidStatus_ShouldUpdateAndReturnSuccess()
        {
            // Arrange
            var login = new AccountLogin { Username = "toshiba", Password = "Solutions" };
            var returnedLogin = await Client.PostAsync<AccountLogin, ResponseBody<LoginResponse>>("account/login", login);
            Client.AddHeader("Authorization", $"Bearer {returnedLogin!.Data!.Token}");

            var edit = new AccountEdit { Status = "Busy" };

            // Act
            var response = await Client.PutAsync<AccountEdit, ResponseBody<int>>("account", edit);

            // Assert
            response.Should().NotBeNull();
            response!.Status.Should().Be(200);
            response.Title.Should().Be("Updated");

            // Confirm the change persisted
            var account = await Client.GetAsync<ResponseBody<AccountRetrieve>>("account");
            account!.Data!.Status.Should().Be("Busy");

            // Restore original status
            await Client.PutAsync<AccountEdit, ResponseBody<int>>("account", new AccountEdit { Status = "Ready to chat" });
        }

        [Test]
        public async Task EditAccount_WithoutToken_ShouldReturnUnauthorized()
        {
            // Arrange — no Authorization header added
            var edit = new AccountEdit { Name = "Should Fail" };

            // Act
            var response = await Client.PutAsync<AccountEdit, ResponseBody<int>>("account", edit);

            // Assert
            response.Should().BeNull(); // 401 returns empty body
        }
    }
}
