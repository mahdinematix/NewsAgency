using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NewsAgency.Application.ArticleServices.CommandHandlers;
using NewsAgency.Domain.ArticleAgg.Repositories;
using NewsAgency.Domain.AuthorAgg.Repositories;
using NewsAgency.Infra.Data.EFCore;
using NewsAgency.Infra.Data.EFCore.CommandRepositories;
using NewsAgency.Infra.Data.EFCore.QueryRepositories;

namespace NewsAgency.Infra.Configuration
{
    public static class Bootstrapper
    {
        public static IServiceCollection WireUp(this IServiceCollection services, string connectionString)
        {
            services.AddScoped<IArticleCommandRepository, ArticleCommandRepository>();
            services.AddScoped<IArticleQueryRepository, ArticleQueryRepository>();

            services.AddScoped<IAuthorCommandRepository, AuthorCommandRepository>();
            services.AddScoped<IAuthorQueryRepository, AuthorQueryRepository>();

            services.AddDbContext<NewsAgencyContext>(x => x.UseSqlServer(connectionString));

            services.AddMediatR(x => x.RegisterServicesFromAssembly(typeof(CreateArticleCommandHandler).Assembly));

            return services;

        }
    }
}
