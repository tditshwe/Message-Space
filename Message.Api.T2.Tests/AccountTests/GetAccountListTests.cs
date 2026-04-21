using MessageApi.Models;
using FluentAssertions;
using Message.Api.T2.Tests.Tools;

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
            var returnedLogin = await Client.PostAsync<AccountLogin, LoginResponse>("account/login", accountLogin);
            Client!.AddHeader("Authorization", $"Bearer {returnedLogin!.Token}");
            var returnedAccountList = await Client.GetAsync<List<Account>>("Account/AccountList");

            // Assert
            returnedAccountList.Count.Should().BeGreaterThan(0);
        }
    }
}
