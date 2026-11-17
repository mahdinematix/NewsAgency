namespace NewsAgency.EndPoints.WebApi.Tests.Integration.Infrastructure;

[CollectionDefinition("IntegrationTests", DisableParallelization = true)]
public class IntegrationTestCollection
    : ICollectionFixture<TestDatabaseFixture>
{
}