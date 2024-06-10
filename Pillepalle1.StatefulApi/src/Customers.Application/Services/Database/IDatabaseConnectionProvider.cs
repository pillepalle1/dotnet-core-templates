namespace Customers.Application.Services.Database;

internal interface IDatabaseConnectionProvider
{
    Task<IDbConnection> ProvideAsync();
}