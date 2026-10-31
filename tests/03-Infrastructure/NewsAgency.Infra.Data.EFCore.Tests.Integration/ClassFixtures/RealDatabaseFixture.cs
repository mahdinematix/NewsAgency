using Microsoft.EntityFrameworkCore;

namespace NewsAgency.Infra.Data.EFCore.Tests.Integration.ClassFixtures;

public class RealDatabaseFixture
{
    private readonly DbContextOptions<NewsAgencyContext> _options;

    public RealDatabaseFixture()
    {
        var connectionString =
            "Data Source=.; Initial Catalog=NewsAgencyDb; Integrated Security=True; TrustServerCertificate=True;";

        _options = new DbContextOptionsBuilder<NewsAgencyContext>()
            .UseSqlServer(connectionString)
            .Options;
    }

    public NewsAgencyContext CreateContext()
    {
        return new NewsAgencyContext(_options);
    }
}