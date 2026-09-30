using System.ComponentModel.DataAnnotations;

namespace PhungDangTruongMVC.Models;

public class AdminSettings
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}

public class LoginViewModel
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";
}

public class ConfirmDeleteViewModel
{
    public string Controller { get; set; } = "";
    public string Id { get; set; } = "";
    public string Message { get; set; } = "";
    public string? Error { get; set; }
}

public class ReportViewModel
{
    [Required(ErrorMessage = "Start date is required.")]
    [DataType(DataType.Date)]
    [Display(Name = "Start Date")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "End date is required.")]
    [DataType(DataType.Date)]
    [Display(Name = "End Date")]
    public DateTime EndDate { get; set; }

    public List<BusinessObjects.NewsArticle>? Articles { get; set; }
}
