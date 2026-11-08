namespace NewsAgency.Application.Contracts.Article.Common;

public class ArticleViewModel
{
    public long Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public long AuthorId { get; set; }
    public long CategoryId { get; set; }
    public string Status { get; set; }
    public ArticleActionsViewModel Actions { get; set; } = new();
}
