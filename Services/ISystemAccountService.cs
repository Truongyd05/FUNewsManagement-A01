using BusinessObjects;

namespace Services;

public interface ISystemAccountService
{
    List<SystemAccount> GetAll();
    SystemAccount? GetById(short id);
    SystemAccount? Login(string email, string password);
    List<SystemAccount> Search(string? keyword, int? role);
    bool EmailExists(string email, short? excludeId = null);

    /// <exception cref="InvalidOperationException">Business rule violated.</exception>
    void Add(SystemAccount account);

    /// <exception cref="InvalidOperationException">Business rule violated.</exception>
    void Update(SystemAccount account);

    /// <exception cref="InvalidOperationException">Account still owns news articles.</exception>
    void Delete(short id);
}
