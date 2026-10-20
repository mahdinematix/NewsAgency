namespace NewsAgency.Application.Contracts.Common;

public class ArticleViewModel
{
    public long Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public long AuthorId { get; set; }
    public string AuthorName { get; set; }
    public long CategoryId { get; set; }
    public string CategoryTitle { get; set; }
    public string Status { get; set; }


}
