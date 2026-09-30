using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhungDangTruongMVC.Models;
using Services;

namespace PhungDangTruongMVC.Controllers;

/// <summary>Admin: statistic report by created-date period, newest first.</summary>
[Authorize(Roles = "Admin")]
public class ReportsController : Controller
{
    private readonly INewsArticleService _news;

    public ReportsController(INewsArticleService news)
    {
        _news = news;
    }

    public IActionResult Index(DateTime? startDate, DateTime? endDate)
    {
        var today = DateTime.Today;
        var model = new ReportViewModel
        {
            StartDate = startDate ?? new DateTime(today.Year, today.Month, 1),
            EndDate = endDate ?? today
        };

        if (startDate.HasValue && endDate.HasValue)
        {
            try
            {
                model.Articles = _news.GetReport(model.StartDate, model.EndDate);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }
        return View(model);
    }
}
