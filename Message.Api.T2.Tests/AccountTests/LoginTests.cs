using MessageApi.Models;
using FluentAssertions;

namespace Message.Api.T2.Tests.AccountTests
{
	internal class LoginTests: TestBase
	{
		[Test]
		public async Task Login_GivenWrongCredentials_ShouldReturnBadRequest()
		{
			// Arrange
			var login = new AccountLogin { Username = "toshiba", Password = "wrongpass" };

			// Act
			var response = await Client.PostAsync<AccountLogin, ResponseBody<LoginResponse>>("account/login", login);

			// Assert
			response.Should().NotBeNull();
			response!.Status.Should().Be(401);
			response.Message.Should().Be("Invalid login details");
			response.Data.Should().BeNull();
		}

		[Test]
		public async Task Login_GivenCorrectCredentials_ShouldLoginAndReturnAccount()
		{
			// Arrange
			var login = new AccountLogin { Username = "toshiba", Password = "Solutions" };

			// Act
			var response = await Client.PostAsync<AccountLogin, ResponseBody<LoginResponse>>("account/login", login);

			// Assert
			response.Should().NotBeNull();
			response!.Status.Should().Be(200);
			response.Data!.Username.Should().Be("toshiba");
			response.Data.Name.Should().NotBeNullOrEmpty();
			response.Data.Token.Should().NotBeNullOrEmpty();
			response.Data.Status.Should().NotBeNullOrEmpty();
		}
	}
}
