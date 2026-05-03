using Azure;
using FluentAssertions;
using Message.Api.T2.Tests.Tools;
using MessageApi.Models;

namespace Message.Api.T2.Tests.AccountTests
{
    internal class GetAccountListTests: TestBase
    {
        [Test]
        public async Task GetAccountList_ShouldReturnAccountList()
        {
            // Arrange
            var accountLogin = new AccountLogin { Username = "toshiba", Password = "Solutions" };

            // Act
            var returnedLogin = await Client.PostAsync<AccountLogin, ResponseBody<LoginResponse>>("account/login", accountLogin);
            Client!.AddHeader("Authorization", $"Bearer {returnedLogin!.Data!.Token}");
            var returnedAccountList = await Client.GetAsync<ResponseBody<List<AccountRetrieve>>>("Account/AccountList");

			      // Assert
			      returnedAccountList.Title.Should().Be("Success");
			      returnedAccountList!.Status.Should().Be(200);
			      returnedAccountList.Data!.Count.Should().BeGreaterThan(0);
        }
    }
}
