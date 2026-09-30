using BusinessObjects;

namespace Repositories;

public interface ITagRepository
{
    List<Tag> GetAll();
    Tag? GetById(int id);
    List<Tag> Search(string? keyword);
    bool IsInUse(int id);
    bool NameExists(string name, int? excludeId = null);
    void Add(Tag tag);
    void Update(Tag tag);
    void Delete(int id);
}
