using NewsAgency.Domain.ArticleAgg.Entities;
using NewsAgency.Domain.CategoryAgg.ValueObjects;
using NewsAgency.Domain.Common;
using NewsAgency.Domain.Exceptions;

namespace NewsAgency.Domain.CategoryAgg.Entities;

public class Category : BaseEntity
{
    public CategoryTitle CategoryTitle { get; private set; }
    public long? ParentId { get; private set; }
    public bool IsRemoved { get; private set; }
    public ICollection<Article> Articles { get; private set; }
    public ICollection<Category> Children { get; private set; }

    public Category(CategoryTitle categoryTitle, long? parentId = null)
    {
        CategoryTitle = categoryTitle;
        ParentId = parentId;
        IsRemoved = false;
        Articles = new List<Article>();
        Children = new List<Category>();
    }

    public void ChangeTitle(CategoryTitle categoryTitle)
    {
        CategoryTitle = categoryTitle;
    }

    public void ChangeParent(long? parentId)
    { 
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

