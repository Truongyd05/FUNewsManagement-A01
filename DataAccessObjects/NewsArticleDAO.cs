using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects;

public sealed class NewsArticleDAO
{
    private static NewsArticleDAO? _instance;
    private static readonly object _lock = new();

    private NewsArticleDAO() { }

    public static NewsArticleDAO Instance
    {
        get
        {
            lock (_lock)
            {
                return _instance ??= new NewsArticleDAO();
            }
        }
    }

    private static IQueryable<NewsArticle> WithDetails(FUNewsManagementContext db) =>
        db.NewsArticles.AsNoTracking()
            .Include(n => n.Category)
            .Include(n => n.CreatedBy)
            .Include(n => n.Tags);

    public List<NewsArticle> GetAll()
    {
        using var db = new FUNewsManagementContext();
        return WithDetails(db).OrderByDescending(n => n.CreatedDate).ToList();
    }

    public NewsArticle? GetById(string id)
    {
        using var db = new FUNewsManagementContext();
        return WithDetails(db).FirstOrDefault(n => n.NewsArticleId == id);
    }

    /// <param name="activeOnly">true: only active news (public / lecturer view).</param>
    /// <param name="createdById">filter by author (staff history).</param>
    public List<NewsArticle> Search(string? keyword, short? categoryId, bool? status, bool activeOnly, short? createdById)
    {
        using var db = new FUNewsManagementContext();
        var query = WithDetails(db);
        if (activeOnly)
        {
            query = query.Where(n => n.NewsStatus == true);
        }
        else if (status.HasValue)
        {
            query = query.Where(n => n.NewsStatus == status);
        }
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            keyword = keyword.Trim();
            query = query.Where(n => (n.NewsTitle != null && n.NewsTitle.Contains(keyword))
                                  || n.Headline.Contains(keyword)
                                  || (n.NewsContent != null && n.NewsContent.Contains(keyword))
                                  || n.Tags.Any(t => t.TagName != null && t.TagName.Contains(keyword)));
        }
        if (categoryId.HasValue)
        {
            query = query.Where(n => n.CategoryId == categoryId);
        }
        if (createdById.HasValue)
        {
            query = query.Where(n => n.CreatedById == createdById);
        }
        return query.OrderByDescending(n => n.CreatedDate).ThenByDescending(n => n.NewsArticleId).ToList();
    }

    public List<NewsArticle> GetByPeriod(DateTime startDate, DateTime endDate)
    {
        using var db = new FUNewsManagementContext();
        var endExclusive = endDate.Date.AddDays(1);
        return WithDetails(db)
            .Where(n => n.CreatedDate >= startDate.Date && n.CreatedDate < endExclusive)
            .OrderByDescending(n => n.CreatedDate)
            .ThenByDescending(n => n.NewsArticleId)
            .ToList();
    }

    private static string GenerateId(FUNewsManagementContext db)
    {
        var ids = db.NewsArticles.Select(n => n.NewsArticleId).ToList();
        int max = ids.Select(id => int.TryParse(id, out var v) ? v : 0).DefaultIfEmpty(0).Max();
        return (max + 1).ToString();
    }

    public void Add(NewsArticle article, IEnumerable<int> tagIds)
    {
        using var db = new FUNewsManagementContext();
        var ids = tagIds.Distinct().ToList();
        article.NewsArticleId = GenerateId(db);
        article.Category = null;
        article.CreatedBy = null;
        article.Tags = db.Tags.Where(t => ids.Contains(t.TagId)).ToList();
        db.NewsArticles.Add(article);
        db.SaveChanges();
    }

    public void Update(NewsArticle article, IEnumerable<int> tagIds)
    {
        using var db = new FUNewsManagementContext();
        var ids = tagIds.Distinct().ToList();
        var existing = db.NewsArticles.Include(n => n.Tags)
            .FirstOrDefault(n => n.NewsArticleId == article.NewsArticleId)
            ?? throw new InvalidOperationException("News article not found.");

        existing.NewsTitle = article.NewsTitle;
        existing.Headline = article.Headline;
        existing.NewsContent = article.NewsContent;
        existing.NewsSource = article.NewsSource;
        existing.CategoryId = article.CategoryId;
        existing.NewsStatus = article.NewsStatus;
        existing.UpdatedById = article.UpdatedById;
        existing.ModifiedDate = article.ModifiedDate;

        existing.Tags.Clear();
        foreach (var tag in db.Tags.Where(t => ids.Contains(t.TagId)))
        {
            existing.Tags.Add(tag);
        }
        db.SaveChanges();
    }

    public void Delete(string id)
    {
        using var db = new FUNewsManagementContext();
        var existing = db.NewsArticles.Include(n => n.Tags)
            .FirstOrDefault(n => n.NewsArticleId == id)
            ?? throw new InvalidOperationException("News article not found.");
        existing.Tags.Clear();
        db.NewsArticles.Remove(existing);
        db.SaveChanges();
    }
}
