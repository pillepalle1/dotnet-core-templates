namespace Pillepalle1.StatefulApi.Application.Services;

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
            CREATE TABLE IF NOT EXISTS books (
            isbn TEXT PRIMARY KEY,
            title TEXT NOT NULL);
        ");
            
            transaction.Commit();
        }
    }
}