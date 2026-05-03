using MessageApi.Models;
using Microsoft.AspNetCore.Identity;

namespace MessageApi.Database.Interfaces
{
    public interface IAccountDatabase
    {
        Account Find(string username);
        List<Account> GetList(string username);
        void Create(Account account);
        bool IsAthenticated(AccountLogin account);
        Account? GetAthenticated(string? username, string? password);
        void Update(Account account);
    }
}
