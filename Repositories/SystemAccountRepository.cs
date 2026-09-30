using BusinessObjects;
using DataAccessObjects;

namespace Repositories;

public class SystemAccountRepository : ISystemAccountRepository
{
    public List<SystemAccount> GetAll() => SystemAccountDAO.Instance.GetAll();
    public SystemAccount? GetById(short id) => SystemAccountDAO.Instance.GetById(id);
    public SystemAccount? GetByEmail(string email) => SystemAccountDAO.Instance.GetByEmail(email);
    public List<SystemAccount> Search(string? keyword, int? role) => SystemAccountDAO.Instance.Search(keyword, role);
    public bool EmailExists(string email, short? excludeId = null) => SystemAccountDAO.Instance.EmailExists(email, excludeId);
    public bool HasNewsArticles(short id) => SystemAccountDAO.Instance.HasNewsArticles(id);
    public void Add(SystemAccount account) => SystemAccountDAO.Instance.Add(account);
    public void Update(SystemAccount account) => SystemAccountDAO.Instance.Update(account);
    public void Delete(short id) => SystemAccountDAO.Instance.Delete(id);
}
