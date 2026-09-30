using System.ComponentModel.DataAnnotations;

namespace BusinessObjects;

public partial class Category
{
    [Display(Name = "ID")]
    public short CategoryId { get; set; }

    [Required(ErrorMessage = "Category name is required.")]
    [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters.")]
    [Display(Name = "Category Name")]
    public string CategoryName { get; set; } = null!;

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(250, ErrorMessage = "Description cannot exceed 250 characters.")]
    [Display(Name = "Description")]
    public string CategoryDesciption { get; set; } = null!;

    [Display(Name = "Parent Category")]
    public short? ParentCategoryId { get; set; }

    [Display(Name = "Active")]
    public bool? IsActive { get; set; }

    public virtual ICollection<Category> InverseParentCategory { get; set; } = new List<Category>();

    public virtual ICollection<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();

    public virtual Category? ParentCategory { get; set; }
}
