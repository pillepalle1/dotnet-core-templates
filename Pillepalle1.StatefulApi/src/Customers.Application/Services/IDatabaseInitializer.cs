namespace Customers.Application.Services;

public interface IDatabaseInitializer
{
    Task InitializeAsync();
}