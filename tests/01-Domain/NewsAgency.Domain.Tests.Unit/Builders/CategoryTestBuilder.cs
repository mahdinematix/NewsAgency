using NewsAgency.Domain.CategoryAgg.Entities;
using NewsAgency.Domain.CategoryAgg.ValueObjects;

namespace NewsAgency.Domain.Tests.Unit.Builders;

public class CategoryTestBuilder
{
    private long _id = 1;
    private CategoryTitle _title = new(new string('a', 2));

    private long? _parentId = null;


    public Category Build()
    {
        return new Category(_id, _title, _parentId);
    }

    public CategoryTestBuilder WithId(long id)
    {
        _id = id;
        return this;
    }
    public CategoryTestBuilder WithTitle(CategoryTitle title)
    {
        _title = title;
        return this;
    }
    public CategoryTestBuilder WithParentId(long? parentId)
    {
        _parentId = parentId;
        return this;
    }
}

