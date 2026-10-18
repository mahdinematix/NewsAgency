using Microsoft.EntityFrameworkCore;
using NewsAgency.Application.ArticleServices.CommandHandlers;
using NewsAgency.Domain.ArticleAgg.Repositories;
using NewsAgency.Infra.Data.EFCore;
using NewsAgency.Infra.Data.EFCore.CommandRepositories;
using NewsAgency.Infra.Data.EFCore.QueryRepositories;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;
// Add services to the container.
var connectionString = configuration.GetConnectionString("NewsAgencyDb");

services.AddDbContext<NewsAgencyContext>(x=>x.UseSqlServer(connectionString));

services.AddMediatR(x => x.RegisterServicesFromAssembly(typeof(CreateArticleCommandHandler).Assembly));

services.AddScoped<IArticleCommandRepository, ArticleCommandRepository>();
services.AddScoped<IArticleQueryRepository, ArticleQueryRepository>();

services.AddControllers();

services.AddSwaggerGen();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapSwagger();
    app.MapSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
