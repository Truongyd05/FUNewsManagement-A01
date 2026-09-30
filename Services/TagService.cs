using BusinessObjects;
using Repositories;

namespace Services;

public class TagService : ITagService
{
    private readonly ITagRepository _repo;

    public TagService(ITagRepository repo)
    {
        _repo = repo;
    }

    public List<Tag> GetAll() => _repo.GetAll();
    public Tag? GetById(int id) => _repo.GetById(id);
    public List<Tag> Search(string? keyword) => _repo.Search(keyword);

    public void Add(Tag tag)
    {
        tag.TagName = tag.TagName?.Trim();
        if (_repo.NameExists(tag.TagName!))
        {
            throw new InvalidOperationException("A tag with this name already exists.");
        }
        _repo.Add(tag);
    }

    public void Update(Tag tag)
    {
        tag.TagName = tag.TagName?.Trim();
        if (_repo.NameExists(tag.TagName!, tag.TagId))
        {
            throw new InvalidOperationException("A tag with this name already exists.");
        }
        _repo.Update(tag);
    }

    public void Delete(int id)
    {
        if (_repo.IsInUse(id))
        {
            throw new InvalidOperationException("Cannot delete this tag because it is used by news articles.");
        }
        _repo.Delete(id);
    }
}
