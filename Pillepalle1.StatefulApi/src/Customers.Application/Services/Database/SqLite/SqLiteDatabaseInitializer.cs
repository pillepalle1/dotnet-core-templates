namespace Customers.Application.Services.Database.SqLite;

internal class SqLiteDatabaseInitializer(
    IDatabaseConnectionProvider dbConnectionProvider) : IDatabaseInitializer
{
    public async Task InitializeAsync()
    {
        using var dbConnection = await dbConnectionProvider.ProvideAsync();
        using var transaction = dbConnection.BeginTransaction();
        
        _ = await dbConnection.ExecuteAsync(
            """
            CREATE TABLE IF NOT EXISTS Customers (
                Id UUID PRIMARY KEY,
                Name TEXT NOT NULL,
                NameNormalized TEXT NOT NULL,
                Details TEXT NOT NULL);
            """);
            
        transaction.Commit();
    }
}
