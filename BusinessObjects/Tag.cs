using System.ComponentModel.DataAnnotations;

namespace BusinessObjects;

public partial class Tag
{
    [Display(Name = "ID")]
    public int TagId { get; set; }

    [Required(ErrorMessage = "Tag name is required.")]
    [StringLength(50, ErrorMessage = "Tag name cannot exceed 50 characters.")]
    [Display(Name = "Tag Name")]
    public string? TagName { get; set; }

    [StringLength(400, ErrorMessage = "Note cannot exceed 400 characters.")]
    public string? Note { get; set; }

    public virtual ICollection<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
}
