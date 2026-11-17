using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NewsAgency.Infra.Data.EFCore;
using Respawn;
using Respawn.Graph;
using RESTFulSense.Clients;

namespace NewsAgency.EndPoints.WebApi.Tests.Integration.Infrastructure;

public sealed class TestDatabaseFixture : IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;

    private SqlConnection _connection = null!;
    private Respawner _respawner = null!;
    private HttpClient _httpClient = null!;

    public RESTFulApiFactoryClient RestClient { get; private set; } = null!;

    public TestDatabaseFixture()
    {
        _factory = new CustomWebApplicationFactory();
    }

    public async Task InitializeAsync()
    {
        _httpClient = _factory.CreateClient();

        RestClient = new RESTFulApiFactoryClient(_httpClient);

        var configuration = _factory.Services
            .GetRequiredService<IConfiguration>();

        var connectionString = configuration
                                   .GetConnectionString("NewsAgencyDb")
                               ?? throw new InvalidOperationException(
                                   "Test database connection string was not found.");

        var sqlConnectionString =
            new SqlConnectionStringBuilder(connectionString);

        if (!string.Equals(
                sqlConnectionString.InitialCatalog,
                "NewsAgency_IntegrationTests",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Cannot initialize a database other than the Integration Test database.");
        }

        // Apply existing migrations
        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider
                .GetRequiredService<NewsAgencyContext>();

            await dbContext.Database.MigrateAsync();
        }

        _connection = new SqlConnection(connectionString);

        await _connection.OpenAsync();

        _respawner = await Respawner.CreateAsync(
            _connection,
            new RespawnerOptions
            {
                DbAdapter = DbAdapter.SqlServer,

                TablesToIgnore =
                [
                    new Table("dbo", "__EFMigrationsHistory")
                ]
            });

        await _respawner.ResetAsync(_connection);
    }
    public async Task ResetAsync()
    {
        await _respawner.ResetAsync(_connection);
    }

    public async Task DisposeAsync()
    {
        if (_connection is not null)
        {
            await _connection.DisposeAsync();
        }

        _httpClient?.Dispose();

        await _factory.DisposeAsync();
    }
}