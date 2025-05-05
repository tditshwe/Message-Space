using MessageApi.Database;
using MessageHandlingApi.Database.Interfaces;
using MessageApi.Models;
using Microsoft.AspNetCore.Identity;

namespace MessageHandlingApi.Database
{
    public class AccountDatabase
    {
        private readonly MessageContext _context = new MessageContext();

        public AccountDatabase(MessageContext messageContext)
        {
            _context = messageContext;
        }

        public Account Find(string username)
        {
            return _context.Account.Find(username);
        }

        public Account GetAthenticated(string username, string password)
        {
            PasswordHasher<Account> hasher = new PasswordHasher<Account>();

            return _context.Account.SingleOrDefault(x => x.Username == username && hasher.VerifyHashedPassword(x, x.Password, password) == PasswordVerificationResult.Success);
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
    }
}
