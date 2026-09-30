using BusinessObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhungDangTruongMVC.Models;
using Services;

namespace PhungDangTruongMVC.Controllers;

/// <summary>Staff: manage news articles (with tags) and view own news history.</summary>
[Authorize(Roles = "Staff")]
public class NewsArticlesController : Controller
{
    private readonly INewsArticleService _news;
    private readonly ICategoryService _categories;
    private readonly ITagService _tags;

    public NewsArticlesController(INewsArticleService news, ICategoryService categories, ITagService tags)
    {
        _news = news;
        _categories = categories;
        _tags = tags;
    }

    public IActionResult Index(string? keyword, short? categoryId, bool? status)
    {
        FillFilters(keyword, categoryId, status, history: false);
        return View(_news.Search(keyword, categoryId, status, activeOnly: false, createdById: null));
    }

    /// <summary>News created by the signed-in staff member.</summary>
    public IActionResult History(string? keyword, short? categoryId, bool? status)
    {
        FillFilters(keyword, categoryId, status, history: true);
        return View("Index", _news.Search(keyword, categoryId, status, activeOnly: false, createdById: User.GetAccountId()));
    }

    public IActionResult Details(string id)
    {
        var article = _news.GetById(id);
        return article == null ? NotFound() : PartialView("_Details", article);
    }

    public IActionResult Create()
    {
        var article = new NewsArticle { NewsStatus = true };
        FillFormLists(article, new List<int>());
        return PartialView("_Form", article);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Create(NewsArticle model, List<int>? selectedTagIds)
    {
        selectedTagIds ??= new List<int>();
        ModelState.Remove(nameof(NewsArticle.NewsArticleId));
        if (ModelState.IsValid)
        {
            try
            {
                _news.Add(model, selectedTagIds, User.GetAccountId());
                return Json(new { success = true });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }
        FillFormLists(model, selectedTagIds);
        return PartialView("_Form", model);
    }

    public IActionResult Edit(string id)
    {
        var article = _news.GetById(id);
        if (article == null)
        {
            return NotFound();
        }
        FillFormLists(article, article.Tags.Select(t => t.TagId).ToList());
        return PartialView("_Form", article);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Edit(NewsArticle model, List<int>? selectedTagIds)
    {
        selectedTagIds ??= new List<int>();
        if (ModelState.IsValid)
        {
            try
            {
                _news.Update(model, selectedTagIds, User.GetAccountId());
                return Json(new { success = true });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }
        FillFormLists(model, selectedTagIds);
        return PartialView("_Form", model);
    }

    public IActionResult Delete(string id)
    {
        var article = _news.GetById(id);
        if (article == null)
        {
            return NotFound();
        }
        return PartialView("~/Views/Shared/_ConfirmDelete.cshtml", new ConfirmDeleteViewModel
        {
            Controller = "NewsArticles",
            Id = id,
            Message = $"Are you sure you want to delete the news article \"{article.NewsTitle}\"?"
        });
    }

    [HttpPost, ValidateAntiForgeryToken, ActionName("Delete")]
    public IActionResult DeleteConfirmed(string id)
    {
        try
        {
            _news.Delete(id);
            return Json(new { success = true });
        }
        catch (InvalidOperationException ex)
        {
            return PartialView("~/Views/Shared/_ConfirmDelete.cshtml", new ConfirmDeleteViewModel
            {
                Controller = "NewsArticles",
                Id = id,
                Message = "Delete this news article?",
                Error = ex.Message
            });
        }
    }

    private void FillFilters(string? keyword, short? categoryId, bool? status, bool history)
    {
        ViewBag.Keyword = keyword;
        ViewBag.CategoryId = categoryId;
        ViewBag.Status = status;
        ViewBag.IsHistory = history;
        ViewBag.Categories = _categories.GetAll();
    }

    private void FillFormLists(NewsArticle article, List<int> selectedTagIds)
    {
        var categories = _categories.GetActive();
        if (article.CategoryId.HasValue && categories.All(c => c.CategoryId != article.CategoryId))
        {
            var current = _categories.GetById(article.CategoryId.Value);
            if (current != null)
            {
                categories.Add(current);
            }
        }
        ViewBag.Categories = categories;
        ViewBag.Tags = _tags.GetAll();
        ViewBag.SelectedTagIds = selectedTagIds;
    }
}
