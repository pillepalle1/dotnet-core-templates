namespace Pillepalle1.StatefulApi.Application.Services;

public interface IDatabaseInitializer
{
    Task InitializeAsync();
}