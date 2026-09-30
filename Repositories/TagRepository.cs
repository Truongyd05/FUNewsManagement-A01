using BusinessObjects;
using DataAccessObjects;

namespace Repositories;

public class TagRepository : ITagRepository
{
    public List<Tag> GetAll() => TagDAO.Instance.GetAll();
    public Tag? GetById(int id) => TagDAO.Instance.GetById(id);
    public List<Tag> Search(string? keyword) => TagDAO.Instance.Search(keyword);
    public bool IsInUse(int id) => TagDAO.Instance.IsInUse(id);
    public bool NameExists(string name, int? excludeId = null) => TagDAO.Instance.NameExists(name, excludeId);
    public void Add(Tag tag) => TagDAO.Instance.Add(tag);
    public void Update(Tag tag) => TagDAO.Instance.Update(tag);
    public void Delete(int id) => TagDAO.Instance.Delete(id);
}
