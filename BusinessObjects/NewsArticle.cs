using System.ComponentModel.DataAnnotations;

namespace BusinessObjects;

public partial class NewsArticle
{
    [StringLength(20)]
    [Display(Name = "ID")]
    public string NewsArticleId { get; set; } = null!;

    [Required(ErrorMessage = "News title is required.")]
    [StringLength(400, ErrorMessage = "News title cannot exceed 400 characters.")]
    [Display(Name = "Title")]
    public string? NewsTitle { get; set; }

    [Required(ErrorMessage = "Headline is required.")]
    [StringLength(150, ErrorMessage = "Headline cannot exceed 150 characters.")]
    public string Headline { get; set; } = null!;

    [Display(Name = "Created Date")]
    [DataType(DataType.DateTime)]
    public DateTime? CreatedDate { get; set; }

    [Required(ErrorMessage = "News content is required.")]
    [StringLength(4000, ErrorMessage = "News content cannot exceed 4000 characters.")]
    [Display(Name = "Content")]
    public string? NewsContent { get; set; }

    [StringLength(400, ErrorMessage = "News source cannot exceed 400 characters.")]
    [Display(Name = "Source")]
    public string? NewsSource { get; set; }

    [Required(ErrorMessage = "Category is required.")]
    [Display(Name = "Category")]
    public short? CategoryId { get; set; }

    [Display(Name = "Active")]
    public bool? NewsStatus { get; set; }

    [Display(Name = "Created By")]
    public short? CreatedById { get; set; }

    [Display(Name = "Updated By")]
    public short? UpdatedById { get; set; }

    [Display(Name = "Modified Date")]
    [DataType(DataType.DateTime)]
    public DateTime? ModifiedDate { get; set; }

    public virtual Category? Category { get; set; }

    public virtual SystemAccount? CreatedBy { get; set; }

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
