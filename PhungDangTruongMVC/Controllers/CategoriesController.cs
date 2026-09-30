using BusinessObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhungDangTruongMVC.Models;
using Services;

namespace PhungDangTruongMVC.Controllers;

/// <summary>Staff: manage category information.</summary>
[Authorize(Roles = "Staff")]
public class CategoriesController : Controller
{
    private readonly ICategoryService _categories;

    public CategoriesController(ICategoryService categories)
    {
        _categories = categories;
    }

    public IActionResult Index(string? keyword, bool? isActive)
    {
        ViewBag.Keyword = keyword;
        ViewBag.IsActive = isActive;
        ViewBag.Counts = _categories.CountArticlesPerCategory();
        return View(_categories.Search(keyword, isActive));
    }

    public IActionResult Create() => PartialView("_Form", NewForm(new Category { IsActive = true }));

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Create(Category model)
    {
        ModelState.Remove(nameof(Category.CategoryId));
        if (ModelState.IsValid)
        {
            try
            {
                _categories.Add(model);
                return Json(new { success = true });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }
        return PartialView("_Form", NewForm(model));
    }

    public IActionResult Edit(short id)
    {
        var category = _categories.GetById(id);
        return category == null ? NotFound() : PartialView("_Form", NewForm(category));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Edit(Category model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _categories.Update(model);
                return Json(new { success = true });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }
        return PartialView("_Form", NewForm(model));
    }

    public IActionResult Delete(short id)
    {
        var category = _categories.GetById(id);
        if (category == null)
        {
            return NotFound();
        }
        return PartialView("~/Views/Shared/_ConfirmDelete.cshtml", new ConfirmDeleteViewModel
        {
            Controller = "Categories",
            Id = id.ToString(),
            Message = $"Are you sure you want to delete the category \"{category.CategoryName}\"?",
            Error = _categories.IsInUse(id)
                ? "This category is already used (by news articles or as a parent) and cannot be deleted."
                : null
        });
    }

    [HttpPost, ValidateAntiForgeryToken, ActionName("Delete")]
    public IActionResult DeleteConfirmed(short id)
    {
        try
        {
            _categories.Delete(id);
            return Json(new { success = true });
        }
        catch (InvalidOperationException ex)
        {
            return PartialView("~/Views/Shared/_ConfirmDelete.cshtml", new ConfirmDeleteViewModel
            {
                Controller = "Categories",
                Id = id.ToString(),
                Message = "Delete this category?",
                Error = ex.Message
            });
        }
    }

    private Category NewForm(Category category)
    {
        ViewBag.Parents = _categories.GetAll().Where(c => c.CategoryId != category.CategoryId).ToList();
        return category;
    }
}
