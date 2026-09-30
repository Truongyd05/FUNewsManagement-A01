using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects;

public sealed class SystemAccountDAO
{
    private static SystemAccountDAO? _instance;
    private static readonly object _lock = new();

    private SystemAccountDAO() { }

    public static SystemAccountDAO Instance
    {
        get
        {
            lock (_lock)
            {
                return _instance ??= new SystemAccountDAO();
            }
        }
    }

    public List<SystemAccount> GetAll()
    {
        using var db = new FUNewsManagementContext();
        return db.SystemAccounts.AsNoTracking().OrderBy(a => a.AccountId).ToList();
    }

    public SystemAccount? GetById(short id)
    {
        using var db = new FUNewsManagementContext();
        return db.SystemAccounts.AsNoTracking().FirstOrDefault(a => a.AccountId == id);
    }

    public SystemAccount? GetByEmail(string email)
    {
        using var db = new FUNewsManagementContext();
        return db.SystemAccounts.AsNoTracking().FirstOrDefault(a => a.AccountEmail == email);
    }

    public List<SystemAccount> Search(string? keyword, int? role)
    {
        using var db = new FUNewsManagementContext();
        var query = db.SystemAccounts.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            keyword = keyword.Trim();
            query = query.Where(a => (a.AccountName != null && a.AccountName.Contains(keyword))
                                  || (a.AccountEmail != null && a.AccountEmail.Contains(keyword)));
        }
        if (role.HasValue)
        {
            query = query.Where(a => a.AccountRole == role);
        }
        return query.OrderBy(a => a.AccountId).ToList();
    }

    public bool EmailExists(string email, short? excludeId = null)
    {
        using var db = new FUNewsManagementContext();
        return db.SystemAccounts.Any(a => a.AccountEmail == email
                                          && (!excludeId.HasValue || a.AccountId != excludeId.Value));
    }

    public bool HasNewsArticles(short id)
    {
        using var db = new FUNewsManagementContext();
        return db.NewsArticles.Any(n => n.CreatedById == id);
    }

    public void Add(SystemAccount account)
    {
        using var db = new FUNewsManagementContext();
        short nextId = (short)((db.SystemAccounts.Max(a => (short?)a.AccountId) ?? 0) + 1);
        account.AccountId = nextId;
        db.SystemAccounts.Add(account);
        db.SaveChanges();
    }

    public void Update(SystemAccount account)
    {
        using var db = new FUNewsManagementContext();
        var existing = db.SystemAccounts.Find(account.AccountId)
            ?? throw new InvalidOperationException("Account not found.");
        existing.AccountName = account.AccountName;
        existing.AccountEmail = account.AccountEmail;
        existing.AccountRole = account.AccountRole;
        existing.AccountPassword = account.AccountPassword;
        db.SaveChanges();
    }

    public void Delete(short id)
    {
        using var db = new FUNewsManagementContext();
        var existing = db.SystemAccounts.Find(id)
            ?? throw new InvalidOperationException("Account not found.");
        db.SystemAccounts.Remove(existing);
        db.SaveChanges();
    }
}
