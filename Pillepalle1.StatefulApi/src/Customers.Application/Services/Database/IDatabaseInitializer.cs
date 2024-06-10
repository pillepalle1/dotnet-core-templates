namespace Customers.Application.Services.Database;

public interface IDatabaseInitializer
{
    Task InitializeAsync();
}