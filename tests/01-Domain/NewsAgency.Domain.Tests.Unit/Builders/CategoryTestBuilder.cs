using NewsAgency.Domain.CategoryAgg.Entities;
using NewsAgency.Domain.CategoryAgg.ValueObjects;

namespace NewsAgency.Domain.Tests.Unit.Builders;

public class CategoryTestBuilder
{
    private CategoryTitle _categoryTitle = new(new string('a', 2));

    private long? _parentId = null;


    public Category Build()
    {
        return new Category(_categoryTitle, _parentId);
    }

    public CategoryTestBuilder WithTitle(CategoryTitle categoryTitle)
    {
        _categoryTitle = categoryTitle;
        return this;
    }
    public CategoryTestBuilder WithParentId(long? parentId)
    {
        _parentId = parentId;
        return this;
    }
}

