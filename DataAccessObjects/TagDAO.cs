using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects;

public sealed class TagDAO
{
    private static TagDAO? _instance;
    private static readonly object _lock = new();

    private TagDAO() { }

    public static TagDAO Instance
    {
        get
        {
            lock (_lock)
            {
                return _instance ??= new TagDAO();
            }
        }
    }

    public List<Tag> GetAll()
    {
        using var db = new FUNewsManagementContext();
        return db.Tags.AsNoTracking().OrderBy(t => t.TagName).ToList();
    }

    public Tag? GetById(int id)
    {
        using var db = new FUNewsManagementContext();
        return db.Tags.AsNoTracking().FirstOrDefault(t => t.TagId == id);
    }

    public List<Tag> Search(string? keyword)
    {
        using var db = new FUNewsManagementContext();
        var query = db.Tags.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            keyword = keyword.Trim();
            query = query.Where(t => (t.TagName != null && t.TagName.Contains(keyword))
                                  || (t.Note != null && t.Note.Contains(keyword)));
        }
        return query.OrderBy(t => t.TagId).ToList();
    }

    public bool IsInUse(int id)
    {
        using var db = new FUNewsManagementContext();
        return db.NewsArticles.Any(n => n.Tags.Any(t => t.TagId == id));
    }

    public bool NameExists(string name, int? excludeId = null)
    {
        using var db = new FUNewsManagementContext();
        return db.Tags.Any(t => t.TagName != null && t.TagName.Trim() == name.Trim()
                                && (!excludeId.HasValue || t.TagId != excludeId.Value));
    }

    public void Add(Tag tag)
    {
        using var db = new FUNewsManagementContext();
        tag.TagId = (db.Tags.Max(t => (int?)t.TagId) ?? 0) + 1;
        db.Tags.Add(tag);
        db.SaveChanges();
    }

    public void Update(Tag tag)
    {
        using var db = new FUNewsManagementContext();
        var existing = db.Tags.Find(tag.TagId)
            ?? throw new InvalidOperationException("Tag not found.");
        existing.TagName = tag.TagName;
        existing.Note = tag.Note;
        db.SaveChanges();
    }

    public void Delete(int id)
    {
        using var db = new FUNewsManagementContext();
        var existing = db.Tags.Find(id)
            ?? throw new InvalidOperationException("Tag not found.");
        db.Tags.Remove(existing);
        db.SaveChanges();
    }
}
