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
