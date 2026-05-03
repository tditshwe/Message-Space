using MessageApi.Models;
using Microsoft.AspNetCore.Identity;

namespace MessageApi.Database
{
    public static class DataSeeder
    {
        public static void Seed(MessageContext context)
        {
            SeedAccount(context, "toshiba", "Toshiba", "Solutions");
            SeedAccount(context, "test-user", "Test user", "password");
        }

        private static void SeedAccount(MessageContext context, string username, string name, string password)
        {
            if (context.Account.Any(a => a.Username == username))
                return;

            var account = new Account
            {
                Username = username,
                Name = name,
                Role = "User",
                Status = "Ready to chat"
            };

            var hasher = new PasswordHasher<Account>();
            account.Password = hasher.HashPassword(account, password);

            context.Account.Add(account);
            context.SaveChanges();
        }
    }
}
