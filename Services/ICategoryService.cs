using BusinessObjects;

namespace Services;

public interface ICategoryService
{
    List<Category> GetAll();
    List<Category> GetActive();
    Category? GetById(short id);
    List<Category> Search(string? keyword, bool? isActive);
    Dictionary<short, int> CountArticlesPerCategory();
    bool IsInUse(short id);

    /// <exception cref="InvalidOperationException">Business rule violated.</exception>
    void Add(Category category);

    /// <exception cref="InvalidOperationException">Business rule violated.</exception>
    void Update(Category category);

    /// <exception cref="InvalidOperationException">Category belongs to a news article.</exception>
    void Delete(short id);
}
