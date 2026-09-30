using BusinessObjects;

namespace Services;

public interface ITagService
{
    List<Tag> GetAll();
    Tag? GetById(int id);
    List<Tag> Search(string? keyword);
    void Add(Tag tag);
    void Update(Tag tag);
    void Delete(int id);
}
