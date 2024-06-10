namespace Customers.Application.Services.Database.SqLite;

/// <summary>
/// Creates a new SQLiteDatabaseConnection every time the method is called
/// </summary>
internal class SqLiteConnectionProvider(
    IConfiguration config) : IDatabaseConnectionProvider
{
    public async Task<IDbConnection> ProvideAsync()
    {
        var dbConnection = new SqliteConnection(config.GetConnectionString("SQLite"));
        await dbConnection.OpenAsync();
        
        return dbConnection;
    }
}