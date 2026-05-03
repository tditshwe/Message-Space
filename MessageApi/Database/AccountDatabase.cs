using MessageApi.Database;
using MessageApi.Database.Interfaces;
using MessageApi.Models;
using Microsoft.AspNetCore.Identity;

namespace MessageHandlingApi.Database
{
    public class AccountDatabase(MessageContext messageContext) : IAccountDatabase
	  {
        private readonly MessageContext _context = messageContext;

		    public Account Find(string username)
        {
            return _context.Account.Find(username);
        }

        public bool IsAthenticated(AccountLogin account)
        {
			      PasswordHasher<Account> hasher = new PasswordHasher<Account>();
            var getAccount = _context.Account.SingleOrDefault(x => x.Username == account.Username);


						return getAccount != null
              && hasher.VerifyHashedPassword(getAccount, getAccount.Password, account.Password) == PasswordVerificationResult.Success;
        }

        public List<Account> GetList(string username)
        {
            return _context.Account.Where(ac => ac.Username != username).ToList();
        }

        public void Create(Account account)
        {
            _context.Account.Add(account);
            _context.SaveChanges();
        }

        public void Update(Account account)
        {
            _context.Account.Update(account);
            _context.SaveChanges();
        }

        public Account? GetAthenticated(string? username, string? password)
        {
            PasswordHasher<Account> hasher = new PasswordHasher<Account>();
            var account = _context.Account.SingleOrDefault(x => x.Username == username);

            if (account == null || password == null)
                return null;

            return hasher.VerifyHashedPassword(account, account.Password, password) == PasswordVerificationResult.Success
                ? account
                : null;
        }
    }
}
