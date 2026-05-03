using MessageApi.Database.Interfaces;
using MessageApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MessageApi.Managers
{
	public class AccountManager (IAccountDatabase accountDatabase) : IAccountManager
	{
		private readonly IAccountDatabase _accountDatabase = accountDatabase;

		public ResponseBody<AccountRetrieve> GetAccount(string username)
		{
			var account = _accountDatabase.Find(username);

			if (account == null)
				return new ResponseBody<AccountRetrieve>
				{
					Title = "Not Found",
					Status = 404,
					Message = $"Account '{username}' was not found"
				};

			return new ResponseBody<AccountRetrieve>
			{
				Title = "Success",
				Status = 200,
				Data = new AccountRetrieve
				{
					Username = account.Username,
					Name = account.Name,
					Status = account.Status,
					Role = account.Role,
					ImageUrl = account.ImageUrl
				}
			};
		}

		public ResponseBody<List<AccountRetrieve>> GetAccounts(string username)
		{
			var accounts = _accountDatabase.GetList(username);
			List<AccountRetrieve> accList = [];

			accounts.ForEach(
				ac => accList.Add(new AccountRetrieve
				{
					Username = ac.Username,
					Name = ac.Name,
					Status = ac.Status,
					Role = ac.Role,
					ImageUrl = ac.ImageUrl
				})
			);

			return new ResponseBody<List<AccountRetrieve>>
			{
				Title = "Success",
				Status = 200,
				Data = accList
			};
		}

		public ResponseBody<Account> CreateAccount(AccountCreate account)
		{
			var existing = _accountDatabase.Find(account.Username);

			if (existing != null)
				return new ResponseBody<Account>
				{
					Message = "This username is already taken by another person",
					Status = 403,
					Title = "Already Exists"
				};

			PasswordHasher <Account> hasher = new PasswordHasher<Account>();

			Account newAcc = new Account
			{
				Username = account.Username,
				Role = "User",
				Status = "Ready to chat",
				Name = account.Name
			};

			// Hash account password
			string hashed = hasher.HashPassword(newAcc, account.Password);

			newAcc.Password = hashed;

			_accountDatabase.Create(newAcc);

			return new ResponseBody<Account>
			{
				Status = 201,
				Title = "Created",
				Data = newAcc
			};
		}

		public ResponseBody<LoginResponse> Authenticate(AccountLogin login)
		{
			var account = _accountDatabase.GetAthenticated(login.Username, login.Password);

			if (account == null)
				return new ResponseBody<LoginResponse>
				{
					Title = "Unauthorized",
					Status = 401,
					Message = "Invalid login details"
				};

			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes("WhatsApp Messenger Message Handler");

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[]
				{
					new Claim(ClaimTypes.Name, account.Username!),
					new Claim(ClaimTypes.Role, account.Role!)
				}),
				Expires = DateTime.UtcNow.AddDays(7),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);

			return new ResponseBody<LoginResponse>
			{
				Title = "Success",
				Status = 200,
				Data = new LoginResponse
				{
					Username = account.Username,
					Name = account.Name,
					Token = tokenHandler.WriteToken(token),
					Status = account.Status,
					ImageUrl = account.ImageUrl
				}
			};
		}
	}
}
