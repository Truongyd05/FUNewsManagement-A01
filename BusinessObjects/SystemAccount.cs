using System.ComponentModel.DataAnnotations;

namespace BusinessObjects;

public partial class SystemAccount
{
    [Display(Name = "ID")]
    public short AccountId { get; set; }

    [Required(ErrorMessage = "Account name is required.")]
    [StringLength(100, ErrorMessage = "Account name cannot exceed 100 characters.")]
    [Display(Name = "Name")]
    public string? AccountName { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [StringLength(70, ErrorMessage = "Email cannot exceed 70 characters.")]
    [Display(Name = "Email")]
    public string? AccountEmail { get; set; }

    [Required(ErrorMessage = "Role is required.")]
    [Range(1, 2, ErrorMessage = "Role must be Staff (1) or Lecturer (2).")]
    [Display(Name = "Role")]
    public int? AccountRole { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(70, MinimumLength = 2, ErrorMessage = "Password must be 2-70 characters.")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string? AccountPassword { get; set; }

    public virtual ICollection<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
}
