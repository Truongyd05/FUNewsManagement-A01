using BusinessObjects;
using DataAccessObjects;

namespace Repositories;

public class NewsArticleRepository : INewsArticleRepository
{
    public List<NewsArticle> GetAll() => NewsArticleDAO.Instance.GetAll();
    public NewsArticle? GetById(string id) => NewsArticleDAO.Instance.GetById(id);

    public List<NewsArticle> Search(string? keyword, short? categoryId, bool? status, bool activeOnly, short? createdById)
        => NewsArticleDAO.Instance.Search(keyword, categoryId, status, activeOnly, createdById);

    public List<NewsArticle> GetByPeriod(DateTime startDate, DateTime endDate)
        => NewsArticleDAO.Instance.GetByPeriod(startDate, endDate);

    public void Add(NewsArticle article, IEnumerable<int> tagIds) => NewsArticleDAO.Instance.Add(article, tagIds);
    public void Update(NewsArticle article, IEnumerable<int> tagIds) => NewsArticleDAO.Instance.Update(article, tagIds);
    public void Delete(string id) => NewsArticleDAO.Instance.Delete(id);
}
