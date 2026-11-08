namespace NewsAgency.Application.Contracts.Article.Common;

public class ArticleActionsViewModel
{
    public bool CanEdit { get; set; }

    public bool CanPublish { get; set; }

    public bool CanReject { get; set; }

    public bool CanArchive { get; set; }

    public bool CanMoveToDraft { get; set; }
}