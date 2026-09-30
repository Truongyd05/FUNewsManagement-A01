using BusinessObjects;

namespace Services;

public interface INewsArticleService
{
    NewsArticle? GetById(string id);
    List<NewsArticle> Search(string? keyword, short? categoryId, bool? status, bool activeOnly, short? createdById);
    List<NewsArticle> GetReport(DateTime startDate, DateTime endDate);
    void Add(NewsArticle article, IEnumerable<int> tagIds, short currentAccountId);
    void Update(NewsArticle article, IEnumerable<int> tagIds, short currentAccountId);
    void Delete(string id);
}
