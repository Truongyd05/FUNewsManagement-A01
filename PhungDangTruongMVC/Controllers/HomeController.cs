using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PhungDangTruongMVC.Models;
using Services;

namespace PhungDangTruongMVC.Controllers;

/// <summary>Public news view: no authentication required, only active news is shown.</summary>
public class HomeController : Controller
{
    private readonly INewsArticleService _news;
    private readonly ICategoryService _categories;

    public HomeController(INewsArticleService news, ICategoryService categories)
    {
        _news = news;
        _categories = categories;
    }

    public IActionResult Index(string? keyword, short? categoryId)
    {
        ViewBag.Keyword = keyword;
        ViewBag.CategoryId = categoryId;
        ViewBag.Categories = _categories.GetActive();

        // Counts for the sidebar are always based on the full active feed,
        // so filtering by one category does not zero-out the other counters.
        var activeArticles = _news.Search(null, null, null, activeOnly: true, createdById: null);
        ViewBag.CategoryCounts = activeArticles
            .Where(n => n.CategoryId.HasValue)
            .GroupBy(n => n.CategoryId!.Value)
            .ToDictionary(g => g.Key, g => g.Count());
        ViewBag.TotalCount = activeArticles.Count;

        // Sidebar rails read from the full active feed too, so they stay
        // populated even when the current filter returns nothing.
        ViewBag.TopStories = activeArticles.Take(5).ToList();
        ViewBag.TrendingTags = activeArticles
            .SelectMany(n => n.Tags)
            .Where(t => !string.IsNullOrWhiteSpace(t.TagName))
            .GroupBy(t => t.TagName!)
            .OrderByDescending(g => g.Count())
            .ThenBy(g => g.Key)
            .Take(12)
            .Select(g => g.Key)
            .ToList();

        return View(_news.Search(keyword, categoryId, null, activeOnly: true, createdById: null));
    }

    public IActionResult Details(string id)
    {
        var article = _news.GetById(id);
        if (article == null || article.NewsStatus != true)
        {
            return NotFound();
        }
        return View(article);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
