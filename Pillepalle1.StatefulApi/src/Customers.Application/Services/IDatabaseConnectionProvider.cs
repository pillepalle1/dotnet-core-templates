namespace Customers.Application.Services;

internal interface IDatabaseConnectionProvider
{
    Task<IDbConnection> ProvideAsync();
}