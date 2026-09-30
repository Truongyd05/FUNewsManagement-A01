using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects;

public sealed class CategoryDAO
{
    private static CategoryDAO? _instance;
    private static readonly object _lock = new();

    private CategoryDAO() { }

    public static CategoryDAO Instance
    {
        get
        {
            lock (_lock)
            {
                return _instance ??= new CategoryDAO();
            }
        }
    }

    public List<Category> GetAll()
    {
        using var db = new FUNewsManagementContext();
        return db.Categories.AsNoTracking()
            .Include(c => c.ParentCategory)
            .OrderBy(c => c.CategoryId)
            .ToList();
    }

    public List<Category> GetActive()
    {
        using var db = new FUNewsManagementContext();
        return db.Categories.AsNoTracking()
            .Where(c => c.IsActive == true)
            .OrderBy(c => c.CategoryName)
            .ToList();
    }

    public Category? GetById(short id)
    {
        using var db = new FUNewsManagementContext();
        return db.Categories.AsNoTracking()
            .Include(c => c.ParentCategory)
            .FirstOrDefault(c => c.CategoryId == id);
    }

    public List<Category> Search(string? keyword, bool? isActive)
    {
        using var db = new FUNewsManagementContext();
        var query = db.Categories.AsNoTracking().Include(c => c.ParentCategory).AsQueryable();
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            keyword = keyword.Trim();
            query = query.Where(c => c.CategoryName.Contains(keyword) || c.CategoryDesciption.Contains(keyword));
        }
        if (isActive.HasValue)
        {
            query = query.Where(c => c.IsActive == isActive);
        }
        return query.OrderBy(c => c.CategoryId).ToList();
    }

    public Dictionary<short, int> CountArticlesPerCategory()
    {
        using var db = new FUNewsManagementContext();
        return db.NewsArticles
            .Where(n => n.CategoryId != null)
            .GroupBy(n => n.CategoryId!.Value)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToDictionary(x => x.Key, x => x.Count);
    }

    /// <summary>A category is in use when a news article belongs to it or another category uses it as parent.</summary>
    public bool IsInUse(short id)
    {
        using var db = new FUNewsManagementContext();
        return db.NewsArticles.Any(n => n.CategoryId == id)
            || db.Categories.Any(c => c.ParentCategoryId == id && c.CategoryId != id);
    }

    public bool NameExists(string name, short? excludeId = null)
    {
        using var db = new FUNewsManagementContext();
        return db.Categories.Any(c => c.CategoryName == name
                                      && (!excludeId.HasValue || c.CategoryId != excludeId.Value));
    }

    public void Add(Category category)
    {
        using var db = new FUNewsManagementContext();
        category.ParentCategory = null;
        db.Categories.Add(category);
        db.SaveChanges();
    }

    public void Update(Category category)
    {
        using var db = new FUNewsManagementContext();
        var existing = db.Categories.Find(category.CategoryId)
            ?? throw new InvalidOperationException("Category not found.");
        existing.CategoryName = category.CategoryName;
        existing.CategoryDesciption = category.CategoryDesciption;
        existing.ParentCategoryId = category.ParentCategoryId;
        existing.IsActive = category.IsActive;
        db.SaveChanges();
    }

    public void Delete(short id)
    {
        using var db = new FUNewsManagementContext();
        var existing = db.Categories.Find(id)
            ?? throw new InvalidOperationException("Category not found.");
        db.Categories.Remove(existing);
        db.SaveChanges();
    }
}
