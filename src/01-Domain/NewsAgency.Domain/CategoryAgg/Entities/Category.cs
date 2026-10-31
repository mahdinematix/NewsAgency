using NewsAgency.Domain.ArticleAgg.Entities;
using NewsAgency.Domain.CategoryAgg.ValueObjects;
using NewsAgency.Domain.Common;
using NewsAgency.Domain.Exceptions;

namespace NewsAgency.Domain.CategoryAgg.Entities;

public class Category : BaseEntity
{
    public CategoryTitle Title { get; private set; }
    public long? ParentId { get; private set; }
    public bool IsRemoved { get; private set; }
    public ICollection<Article> Articles { get; private set; }
    public ICollection<Category> Children { get; private set; }

    public Category(CategoryTitle title, long? parentId = null)
    {
        Title = title;
        ParentId = parentId;
        IsRemoved = false;
        Articles = new List<Article>();
        Children = new List<Category>();
    }

    public void ChangeTitle(CategoryTitle categoryTitle)
    {
        Title = categoryTitle;
    }

    public void ChangeParent(long? parentId)
    { 
        ParentId = parentId;
    }

    public void Remove(bool hasActiveChildren)
    {
        if (IsRemoved)
        {
            return;
        }

        if (hasActiveChildren)
        {
            throw new DomainStateException(
                "A category cannot be removed while it has active child categories.");
        }

        IsRemoved = true;
    }
    public void Restore(bool parentIsRemoved)
    {
        if (!IsRemoved)
        {
            return;
        }

        if (parentIsRemoved)
        {
            throw new DomainStateException(
                "A category cannot be restored while its parent category is removed.");
        }

        IsRemoved = false;
    }
}

