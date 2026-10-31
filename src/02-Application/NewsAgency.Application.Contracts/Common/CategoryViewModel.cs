namespace NewsAgency.Application.Contracts.Common;

public class CategoryViewModel
{
    public long Id { get; set; }
    public string Title { get; set; }
    public long? ParentId { get; set; }
    public bool IsRemoved { get; set; }
}