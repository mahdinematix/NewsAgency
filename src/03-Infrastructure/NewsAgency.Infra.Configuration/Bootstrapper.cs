using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NewsAgency.Application.ArticleServices.CommandHandlers.Create;
using NewsAgency.Application.Common;
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

            var applicationLayerAssembly = typeof(CreateArticleCommandHandler).Assembly;

            services.AddMediatR(x =>
            {
                x.RegisterServicesFromAssembly(applicationLayerAssembly);
                x.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            services.AddValidatorsFromAssembly(applicationLayerAssembly);


            return services;

        }
    }
}
