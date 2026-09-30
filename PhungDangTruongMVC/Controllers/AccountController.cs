using System.Security.Claims;
using BusinessObjects;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PhungDangTruongMVC.Models;
using Services;

namespace PhungDangTruongMVC.Controllers;

public class AccountController : Controller
{
    private readonly ISystemAccountService _accounts;
    private readonly AdminSettings _admin;

    public AccountController(ISystemAccountService accounts, IOptions<AdminSettings> admin)
    {
        _accounts = accounts;
        _admin = admin.Value;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToHome(User.GetRole());
        }
        return View(new LoginViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        string id, name, role;
        var email = model.Email.Trim();
        if (string.Equals(email, _admin.Email, StringComparison.OrdinalIgnoreCase) && model.Password == _admin.Password)
        {
            id = "0";
            name = "Administrator";
            role = "Admin";
        }
        else
        {
            var account = _accounts.Login(email, model.Password);
            if (account == null)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }
            id = account.AccountId.ToString();
            name = account.AccountName ?? account.AccountEmail ?? "";
            role = AccountRole.GetName(account.AccountRole);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, id),
            new(ClaimTypes.Name, name),
            new(ClaimTypes.Email, email),
            new(ClaimTypes.Role, role)
        };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        return RedirectToHome(role);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    public IActionResult AccessDenied() => View();

    // ---- Staff: manage own profile ----

    [Authorize(Roles = "Staff")]
    public IActionResult Profile()
    {
        var account = _accounts.GetById(User.GetAccountId());
        return account == null ? NotFound() : View(account);
    }

    [Authorize(Roles = "Staff")]
    public IActionResult EditProfile()
    {
        var account = _accounts.GetById(User.GetAccountId());
        return account == null ? NotFound() : PartialView("_ProfileForm", account);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "Staff")]
    public async Task<IActionResult> EditProfile(SystemAccount model)
    {
        var current = _accounts.GetById(User.GetAccountId());
        if (current == null)
        {
            return NotFound();
        }
        // A staff member cannot change id or role.
        model.AccountId = current.AccountId;
        model.AccountRole = current.AccountRole;
        ModelState.Remove(nameof(SystemAccount.AccountRole));

        if (string.Equals(model.AccountEmail?.Trim(), _admin.Email, StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(nameof(SystemAccount.AccountEmail), "This email is reserved.");
        }
        if (ModelState.IsValid)
        {
            try
            {
                _accounts.Update(model);
                // refresh the display name / email held in the cookie
                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, model.AccountId.ToString()),
                    new(ClaimTypes.Name, model.AccountName ?? ""),
                    new(ClaimTypes.Email, model.AccountEmail ?? ""),
                    new(ClaimTypes.Role, "Staff")
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));
                return Json(new { success = true });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }
        return PartialView("_ProfileForm", model);
    }

    private IActionResult RedirectToHome(string? role) => role switch
    {
        "Admin" => RedirectToAction("Index", "Accounts"),
        "Staff" => RedirectToAction("Index", "NewsArticles"),
        _ => RedirectToAction("Index", "Home")
    };
}
