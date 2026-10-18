using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NewsAgency.Domain.ArticleAgg.Entities;
using NewsAgency.Domain.AuthorAgg.Entities;
using NewsAgency.Domain.CategoryAgg.Entities;

namespace NewsAgency.Infra.Data.EFCore.Configurations;

public class ArticleConfiguration : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
        builder.ToTable("Articles");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Status).HasConversion<int>().IsRequired();

        builder.HasOne<Author>().WithMany(x => x.Articles).HasForeignKey(x => x.AuthorId);

        builder.HasOne<Category>().WithMany(x => x.Articles).HasForeignKey(x => x.CategoryId);

    }
}