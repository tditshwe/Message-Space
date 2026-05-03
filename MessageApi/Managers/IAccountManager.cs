using MessageApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace MessageApi.Managers
{
	public interface IAccountManager
	{
		ResponseBody<AccountRetrieve> GetAccount(string username);
		ResponseBody<List<AccountRetrieve>> GetAccounts(string username);
		ResponseBody<Account> CreateAccount(AccountCreate account);
		ResponseBody<LoginResponse> Authenticate(AccountLogin login);
	}
}
