using BusinessObjects;
using Repositories;

namespace Services;

public class NewsArticleService : INewsArticleService
{
    private readonly INewsArticleRepository _repo;

    public NewsArticleService(INewsArticleRepository repo)
    {
        _repo = repo;
    }

    public NewsArticle? GetById(string id) => _repo.GetById(id);

    public List<NewsArticle> Search(string? keyword, short? categoryId, bool? status, bool activeOnly, short? createdById)
        => _repo.Search(keyword, categoryId, status, activeOnly, createdById);

    public List<NewsArticle> GetReport(DateTime startDate, DateTime endDate)
    {
        if (startDate.Date > endDate.Date)
        {
            throw new InvalidOperationException("Start date must not be later than end date.");
        }
        return _repo.GetByPeriod(startDate, endDate);
    }

    public void Add(NewsArticle article, IEnumerable<int> tagIds, short currentAccountId)
    {
        article.CreatedById = currentAccountId;
        article.UpdatedById = currentAccountId;
        article.CreatedDate = DateTime.Now;
        article.ModifiedDate = article.CreatedDate;
        _repo.Add(article, tagIds);
    }

    public void Update(NewsArticle article, IEnumerable<int> tagIds, short currentAccountId)
    {
        if (_repo.GetById(article.NewsArticleId) == null)
        {
            throw new InvalidOperationException("News article not found.");
        }
        article.UpdatedById = currentAccountId;
        article.ModifiedDate = DateTime.Now;
        _repo.Update(article, tagIds);
    }

    public void Delete(string id)
    {
        if (_repo.GetById(id) == null)
        {
            throw new InvalidOperationException("News article not found.");
        }
        _repo.Delete(id);
    }
}
