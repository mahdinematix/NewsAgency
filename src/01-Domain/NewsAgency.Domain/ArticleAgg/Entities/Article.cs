using NewsAgency.Domain.ArticleAgg.ValueObjects;
using NewsAgency.Domain.Common;
using NewsAgency.Domain.Exceptions;

namespace NewsAgency.Domain.ArticleAgg.Entities;

public class Article : BaseEntity
{
    public ArticleTitle Title { get; private set; }
    public ArticleContent Content { get; private set; }
    public long AuthorId { get; private set; }
    public long CategoryId { get; private set; }
    public ArticleStatus Status { get; private set; }

    public Article(ArticleTitle title, ArticleContent content, long authorId, long categoryId)
    {
        Title = title;
        Content = content;
        AuthorId = authorId;
        CategoryId = categoryId;
        Status = ArticleStatus.Draft;
    }
    public bool CanEdit =>
        Status == ArticleStatus.Draft ||
        Status == ArticleStatus.Rejected;

    public bool CanPublish =>
        Status == ArticleStatus.Draft;

    public bool CanReject =>
        Status == ArticleStatus.Draft;

    public bool CanArchive =>
        Status == ArticleStatus.Published;

    public bool CanMoveToDraft =>
        Status == ArticleStatus.Rejected ||
        Status == ArticleStatus.Archived;

    public void Edit(ArticleTitle title, ArticleContent content, long authorId, long categoryId)
    {
        if (!CanEdit)
        {
            throw new DomainStateException(
                Messages.OperationCannotDoneFromThisStatus);
        }

        Title = title;
        Content = content;
        AuthorId = authorId;
        CategoryId = categoryId;
    }

    public void Publish()
    {
        if (Status == ArticleStatus.Published)
        {
            return;
        }

        if (!CanPublish)
        {
            throw new DomainStateException(Messages.StatusCannotChangeFromThisStateToThatState);
        }
        
        Status = ArticleStatus.Published;
    }

    public void Archive()
    {
        if (Status == ArticleStatus.Archived)
        {
            return;
        }

        if (!CanArchive)
        {
            throw new DomainStateException(Messages.StatusCannotChangeFromThisStateToThatState);
        }
        
        Status = ArticleStatus.Archived;
    }

    public void Reject()
    {
        if (Status == ArticleStatus.Rejected)
        {
            return;
        }

        if (!CanReject)
        {
            throw new DomainStateException(Messages.StatusCannotChangeFromThisStateToThatState);
        }

        Status = ArticleStatus.Rejected;

    }

    public void MoveToDraft()
    {
        if (Status == ArticleStatus.Draft)
        {
            return;
        }

        if (!CanMoveToDraft)
        {
            throw new DomainStateException(Messages.StatusCannotChangeFromThisStateToThatState);
        }

        Status = ArticleStatus.Draft;
    }
}