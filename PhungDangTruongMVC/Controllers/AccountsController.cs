using BusinessObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PhungDangTruongMVC.Models;
using Services;

namespace PhungDangTruongMVC.Controllers;

/// <summary>Admin: manage account information.</summary>
[Authorize(Roles = "Admin")]
public class AccountsController : Controller
{
    private readonly ISystemAccountService _accounts;
    private readonly AdminSettings _admin;

    public AccountsController(ISystemAccountService accounts, IOptions<AdminSettings> admin)
    {
        _accounts = accounts;
        _admin = admin.Value;
    }

    public IActionResult Index(string? keyword, int? role)
    {
        ViewBag.Keyword = keyword;
        ViewBag.Role = role;
        return View(_accounts.Search(keyword, role));
    }

    public IActionResult Create() => PartialView("_Form", new SystemAccount { AccountRole = AccountRole.Staff });

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Create(SystemAccount model)
    {
        ModelState.Remove(nameof(SystemAccount.AccountId));
        ValidateReservedEmail(model);
        if (ModelState.IsValid)
        {
            try
            {
                _accounts.Add(model);
                return Json(new { success = true });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }
        return PartialView("_Form", model);
    }

    public IActionResult Edit(short id)
    {
        var account = _accounts.GetById(id);
        return account == null ? NotFound() : PartialView("_Form", account);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Edit(SystemAccount model)
    {
        ValidateReservedEmail(model);
        if (ModelState.IsValid)
        {
            try
            {
                _accounts.Update(model);
                return Json(new { success = true });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }
        return PartialView("_Form", model);
    }

    public IActionResult Delete(short id)
    {
        var account = _accounts.GetById(id);
        if (account == null)
        {
            return NotFound();
        }
        return PartialView("~/Views/Shared/_ConfirmDelete.cshtml", new ConfirmDeleteViewModel
        {
            Controller = "Accounts",
            Id = id.ToString(),
            Message = $"Are you sure you want to delete the account \"{account.AccountName}\" ({account.AccountEmail})?"
        });
    }

    [HttpPost, ValidateAntiForgeryToken, ActionName("Delete")]
    public IActionResult DeleteConfirmed(short id)
    {
        try
        {
            _accounts.Delete(id);
            return Json(new { success = true });
        }
        catch (InvalidOperationException ex)
        {
            return PartialView("~/Views/Shared/_ConfirmDelete.cshtml", new ConfirmDeleteViewModel
            {
                Controller = "Accounts",
                Id = id.ToString(),
                Message = "Delete this account?",
                Error = ex.Message
            });
        }
    }

    private void ValidateReservedEmail(SystemAccount model)
    {
        if (string.Equals(model.AccountEmail?.Trim(), _admin.Email, StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(nameof(SystemAccount.AccountEmail), "This email is reserved.");
        }
    }
}
