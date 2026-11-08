namespace NewsAgency.EndPoints.WebApi.Models.Articles;

public class EditArticleRequest
{
    public string Title { get; set; }

    public string Content { get; set; }

    public long AuthorId { get; set; }

    public long CategoryId { get; set; }
}

