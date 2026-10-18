using Microsoft.EntityFrameworkCore;
using NewsAgency.Domain.ArticleAgg.Entities;
using NewsAgency.Domain.ArticleAgg.ValueObjects;
using NewsAgency.Domain.AuthorAgg.Entities;
using NewsAgency.Domain.AuthorAgg.ValueObjects;
using NewsAgency.Domain.CategoryAgg.Entities;
using NewsAgency.Domain.CategoryAgg.ValueObjects;
using NewsAgency.Infra.Data.EFCore.Configurations;
using NewsAgency.Infra.Data.EFCore.ValueConversions;

namespace NewsAgency.Infra.Data.EFCore;

public class NewsAgencyContext : DbContext
{
    public DbSet<Article> Articles { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Category> Categories { get; set; }

    public NewsAgencyContext(DbContextOptions<NewsAgencyContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var assembly = typeof(CategoryConfiguration).Assembly;
        modelBuilder.ApplyConfigurationsFromAssembly(assembly);
        base.OnModelCreating(modelBuilder);

    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {

        configurationBuilder.Properties<ArticleTitle>().HaveConversion<ArticleTitleConversion>();

        configurationBuilder.Properties<ArticleContent>().HaveConversion<ArticleContentConversion>();

        configurationBuilder.Properties<CategoryTitle>().HaveConversion<CategoryTitleConversion>();

        configurationBuilder.Properties<Email>().HaveConversion<EmailConversion>();

        configurationBuilder.Properties<PhoneNumber>().HaveConversion<PhoneNumberConversion>();

        configurationBuilder.Properties<FirstName>().HaveConversion<FirstNameConversion>();

        configurationBuilder.Properties<LastName>().HaveConversion<LastNameConversion>();

        base.ConfigureConventions(configurationBuilder);
    }
}
