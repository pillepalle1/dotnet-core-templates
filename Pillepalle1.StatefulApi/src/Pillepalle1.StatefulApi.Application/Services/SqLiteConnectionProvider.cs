namespace Pillepalle1.StatefulApi.Application.Services;

internal class SqLiteConnectionProvider : IDatabaseConnectionProvider
{
    private readonly IConfiguration _config;
    private SqliteConnection? _dbConnection;

    public SqLiteConnectionProvider(IConfiguration config)
    {
        _config = config;
    }

    public async Task<IDbConnection> ProvideAsync()
    {
        if (_dbConnection is null)
        {
            _dbConnection = new SqliteConnection(_config.GetConnectionString("SQLite"));
            await _dbConnection.OpenAsync();
        }

        return _dbConnection;
    }
}