using MessageApi.Models;

namespace MessageHandlingApi.Database.Interfaces
{
    public interface IAccountDatabase
    {
        Account Find(string username);
        List<Account> GetList(string username);
        void Create(Account account);
        Account GetAthenticated(string username, string password);
        void Update(Account account);
    }
}
