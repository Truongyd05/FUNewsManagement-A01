using BusinessObjects;

namespace Repositories;

public interface ICategoryRepository
{
    List<Category> GetAll();
    List<Category> GetActive();
    Category? GetById(short id);
    List<Category> Search(string? keyword, bool? isActive);
    Dictionary<short, int> CountArticlesPerCategory();
    bool IsInUse(short id);
    bool NameExists(string name, short? excludeId = null);
    void Add(Category category);
    void Update(Category category);
    void Delete(short id);
}
