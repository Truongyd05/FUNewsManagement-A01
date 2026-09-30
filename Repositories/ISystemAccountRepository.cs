using BusinessObjects;

namespace Repositories;

public interface ISystemAccountRepository
{
    List<SystemAccount> GetAll();
    SystemAccount? GetById(short id);
    SystemAccount? GetByEmail(string email);
    List<SystemAccount> Search(string? keyword, int? role);
    bool EmailExists(string email, short? excludeId = null);
    bool HasNewsArticles(short id);
    void Add(SystemAccount account);
    void Update(SystemAccount account);
    void Delete(short id);
}
