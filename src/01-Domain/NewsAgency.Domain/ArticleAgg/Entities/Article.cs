using NewsAgency.Domain.ArticleAgg.ValueObjects;
using NewsAgency.Domain.Common;
using NewsAgency.Domain.Exceptions;

namespace NewsAgency.Domain.ArticleAgg.Entities;

public class Article
{
    public long Id { get; private set; }
    public ArticleTitle Title { get; private set; }
    public ArticleContent Content { get; private set; }
    public long AuthorId { get; private set; }
    public long CategoryId { get; private set; }
    public ArticleStatus Status { get; private set; }

    public Article(long id, ArticleTitle title, ArticleContent content, long authorId, long categoryId)
    {
        Id = id;
        Title = title;
        Content = content;
        AuthorId = authorId;
        CategoryId = categoryId;
        Status = ArticleStatus.Draft;
    }

    public void Edit(ArticleTitle title, ArticleContent content, long authorId, long categoryId)
    {
        if (Status == ArticleStatus.Published || Status == ArticleStatus.Archived)
        {
            throw new DomainStateException(Messages.OperationCannotDoneFromThisStatus);
        }

        Title = title;
        Content = content;
        AuthorId = authorId;
        CategoryId = categoryId;
    }

    public void Publish()
    {
        if (Status == ArticleStatus.Rejected || Status == ArticleStatus.Archived)
        {
            throw new DomainStateException(Messages.StatusCannotChangeFromThisStateToThatState);

        }
        if (Status == ArticleStatus.Published)
        {
            return;
        }
        Status = ArticleStatus.Published;
    }

    public void Archive()
    {
        if (Status == ArticleStatus.Draft || Status == ArticleStatus.Rejected)
        {
            throw new DomainStateException(Messages.StatusCannotChangeFromThisStateToThatState);
        }
        if (Status == ArticleStatus.Archived)
        {
            return;
        }
        Status = ArticleStatus.Archived;
    }

    public void Reject()
    {
        if (Status == ArticleStatus.Published || Status == ArticleStatus.Archived)
        {
            throw new DomainStateException(Messages.StatusCannotChangeFromThisStateToThatState);

        }

        if (Status == ArticleStatus.Rejected)
        {
            return;
        }
        Status = ArticleStatus.Rejected;

    }

    public void MoveToDraft()
    {
        if (Status == ArticleStatus.Published)
        {
            throw new DomainStateException(Messages.StatusCannotChangeFromThisStateToThatState);
        }

        if (Status == ArticleStatus.Draft)
        {
            return;
        }
        Status = ArticleStatus.Draft;
    }
}