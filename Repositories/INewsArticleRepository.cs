using BusinessObjects;

namespace Repositories;

public interface INewsArticleRepository
{
    List<NewsArticle> GetAll();
    NewsArticle? GetById(string id);
    List<NewsArticle> Search(string? keyword, short? categoryId, bool? status, bool activeOnly, short? createdById);
    List<NewsArticle> GetByPeriod(DateTime startDate, DateTime endDate);
    void Add(NewsArticle article, IEnumerable<int> tagIds);
    void Update(NewsArticle article, IEnumerable<int> tagIds);
    void Delete(string id);
}
