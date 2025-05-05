using MessageApi.Models;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Message.Api.T2.Tests.Tools;

namespace Message.Api.T2.Tests.AccountTests
{
    internal class GetAccountListTests: TestBase
    {
        [Test]
        public async Task GetAccountList_ShouldReturnAccountList()
        {
            // Arrange
            var jwtSettings = AppConfig.GetSection("JwtSettings");
            var token = jwtSettings["Token"];

            // Act
            Client!.AddHeader("Authorization", $"Bearer {token}");
            var returnedAccountList = await Client.GetAsync<List<Account>>("Account/AccountList");

            // Assert
            returnedAccountList.Count.Should().BeGreaterThan(0);
        }
    }
}
