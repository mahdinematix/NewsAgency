using NewsAgency.Domain.ArticleAgg.Entities;
using NewsAgency.Domain.CategoryAgg.ValueObjects;
using NewsAgency.Domain.Common;
using NewsAgency.Domain.Exceptions;

namespace NewsAgency.Domain.CategoryAgg.Entities;

public class Category : BaseEntity
{
    public Title Title { get; private set; }
    public long? ParentId { get; private set; }
    public bool IsRemoved { get; private set; }
    public ICollection<Article> Articles { get; private set; }

    public Category(long id, Title title, long? parentId = null)
    {
        if (id == parentId)
        {
            throw new DomainStateException(Messages.ParentOfCategoryCannotBeItself);
        }
        Id = id;
        Title = title;
        ParentId = parentId;
        IsRemoved = false;
        Articles = new List<Article>();
    }

    public void ChangeTitle(Title title)
    {
        Title = title;
    }

    public void ChangeParent(long? parentId)
    {
        if (Id == parentId)
        {
            throw new DomainStateException(Messages.ParentOfCategoryCannotBeItself);
        }
        ParentId = parentId;
    }

    public void Remove()
    {
        if (IsRemoved)
        {
            return;
        }
        IsRemoved = true;
    }
    public void Restore()
    {
        if (!IsRemoved)
        {
            return;
        }
        IsRemoved = false;
    }
}

