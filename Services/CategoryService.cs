using BusinessObjects;
using Repositories;

namespace Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repo;

    public CategoryService(ICategoryRepository repo)
    {
        _repo = repo;
    }

    public List<Category> GetAll() => _repo.GetAll();
    public List<Category> GetActive() => _repo.GetActive();
    public Category? GetById(short id) => _repo.GetById(id);
    public List<Category> Search(string? keyword, bool? isActive) => _repo.Search(keyword, isActive);
    public Dictionary<short, int> CountArticlesPerCategory() => _repo.CountArticlesPerCategory();
    public bool IsInUse(short id) => _repo.IsInUse(id);

    public void Add(Category category)
    {
        category.CategoryName = category.CategoryName.Trim();
        if (_repo.NameExists(category.CategoryName))
        {
            throw new InvalidOperationException("A category with this name already exists.");
        }
        _repo.Add(category);
    }

    public void Update(Category category)
    {
        category.CategoryName = category.CategoryName.Trim();
        if (_repo.GetById(category.CategoryId) == null)
        {
            throw new InvalidOperationException("Category not found.");
        }
        if (_repo.NameExists(category.CategoryName, category.CategoryId))
        {
            throw new InvalidOperationException("A category with this name already exists.");
        }
        _repo.Update(category);
    }

    public void Delete(short id)
    {
        if (_repo.GetById(id) == null)
        {
            throw new InvalidOperationException("Category not found.");
        }
        if (_repo.IsInUse(id))
        {
            throw new InvalidOperationException("Cannot delete this category because it is used by news articles or other categories.");
        }
        _repo.Delete(id);
    }
}
