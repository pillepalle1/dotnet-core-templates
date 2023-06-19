namespace Pillepalle1.StatefulApi.Application.Services;

internal interface IDatabaseConnectionProvider
{
    Task<IDbConnection> ProvideAsync();
}