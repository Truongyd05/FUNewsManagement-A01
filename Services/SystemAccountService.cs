using BusinessObjects;
using Repositories;

namespace Services;

public class SystemAccountService : ISystemAccountService
{
    private readonly ISystemAccountRepository _repo;

    public SystemAccountService(ISystemAccountRepository repo)
    {
        _repo = repo;
    }

    public List<SystemAccount> GetAll() => _repo.GetAll();

    public SystemAccount? GetById(short id) => _repo.GetById(id);

    public SystemAccount? Login(string email, string password)
    {
        var account = _repo.GetByEmail(email.Trim());
        return account != null && account.AccountPassword == password ? account : null;
    }

    public List<SystemAccount> Search(string? keyword, int? role) => _repo.Search(keyword, role);

    public bool EmailExists(string email, short? excludeId = null) => _repo.EmailExists(email.Trim(), excludeId);

    public void Add(SystemAccount account)
    {
        account.AccountEmail = account.AccountEmail?.Trim();
        account.AccountName = account.AccountName?.Trim();
        if (_repo.EmailExists(account.AccountEmail!))
        {
            throw new InvalidOperationException("This email is already used by another account.");
        }
        _repo.Add(account);
    }

    public void Update(SystemAccount account)
    {
        account.AccountEmail = account.AccountEmail?.Trim();
        account.AccountName = account.AccountName?.Trim();
        if (_repo.GetById(account.AccountId) == null)
        {
            throw new InvalidOperationException("Account not found.");
        }
        if (_repo.EmailExists(account.AccountEmail!, account.AccountId))
        {
            throw new InvalidOperationException("This email is already used by another account.");
        }
        _repo.Update(account);
    }

    public void Delete(short id)
    {
        if (_repo.GetById(id) == null)
        {
            throw new InvalidOperationException("Account not found.");
        }
        if (_repo.HasNewsArticles(id))
        {
            throw new InvalidOperationException("Cannot delete this account because it has created news articles.");
        }
        _repo.Delete(id);
    }
}
