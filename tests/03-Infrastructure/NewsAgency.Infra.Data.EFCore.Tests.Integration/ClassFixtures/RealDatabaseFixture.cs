using System.Transactions;
using Microsoft.EntityFrameworkCore;

namespace NewsAgency.Infra.Data.EFCore.Tests.Integration.ClassFixtures;

public class RealDatabaseFixture : IDisposable
{
    public NewsAgencyContext Context;
    private readonly TransactionScope _scope;
    public RealDatabaseFixture()
    {
        var connectionString =
            "Data Source=.; Initial Catalog=NewsAgencyDb; Integrated Security=True; TrustServerCertificate=True;";
        var options = new DbContextOptionsBuilder<NewsAgencyContext>()
            .UseSqlServer(connectionString)
            .Options;
        _scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);
        Context = new NewsAgencyContext(options);

    }
    public void Dispose()
    {
        _scope.Dispose();
        Context.Database.ExecuteSqlRawAsync("truncate table [NewsAgencyDb].[dbo].[Articles]");
        Context.Database.ExecuteSqlRawAsync("truncate table [NewsAgencyDb].[dbo].[Categories]");
        Context.Database.ExecuteSqlRawAsync("truncate table [NewsAgencyDb].[dbo].[Authors]");
        Context.SaveChangesAsync();
        Context.DisposeAsync();

    }
}

