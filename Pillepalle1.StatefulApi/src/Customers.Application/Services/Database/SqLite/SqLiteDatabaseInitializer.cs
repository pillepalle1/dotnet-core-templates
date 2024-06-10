namespace Customers.Application.Services.Database.SqLite;

internal class SqLiteDatabaseInitializer : IDatabaseInitializer
{
    private readonly IDatabaseConnectionProvider _databaseConnectionProvider;

    public SqLiteDatabaseInitializer(IDatabaseConnectionProvider databaseConnectionProvider)
    {
        _databaseConnectionProvider = databaseConnectionProvider;
    }

    public async Task InitializeAsync()
    {
        using (var dbConnection = await _databaseConnectionProvider.ProvideAsync())
        using (var transaction = dbConnection.BeginTransaction())
        {
            _ = await dbConnection.ExecuteAsync(@"
            CREATE TABLE IF NOT EXISTS Customers (
                Id UUID PRIMARY KEY,
                Name TEXT UNIQUE NOT NULL,
                Details TEXT NOT NULL);");
            
            transaction.Commit();
        }
    }
}
