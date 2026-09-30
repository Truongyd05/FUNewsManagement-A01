using BusinessObjects;
using DataAccessObjects;

namespace Repositories;

public class CategoryRepository : ICategoryRepository
{
    public List<Category> GetAll() => CategoryDAO.Instance.GetAll();
    public List<Category> GetActive() => CategoryDAO.Instance.GetActive();
    public Category? GetById(short id) => CategoryDAO.Instance.GetById(id);
    public List<Category> Search(string? keyword, bool? isActive) => CategoryDAO.Instance.Search(keyword, isActive);
    public Dictionary<short, int> CountArticlesPerCategory() => CategoryDAO.Instance.CountArticlesPerCategory();
    public bool IsInUse(short id) => CategoryDAO.Instance.IsInUse(id);
    public bool NameExists(string name, short? excludeId = null) => CategoryDAO.Instance.NameExists(name, excludeId);
    public void Add(Category category) => CategoryDAO.Instance.Add(category);
    public void Update(Category category) => CategoryDAO.Instance.Update(category);
    public void Delete(short id) => CategoryDAO.Instance.Delete(id);
}
