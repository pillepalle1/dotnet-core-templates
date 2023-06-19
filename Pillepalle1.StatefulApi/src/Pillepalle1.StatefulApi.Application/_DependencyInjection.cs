using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Pillepalle1.StatefulApi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Automagically add services via assembly scanning
        var executingAssembly = Assembly.GetExecutingAssembly();
        services.AddValidatorsFromAssembly(executingAssembly);
        services.AddMediatR(executingAssembly);

        // Manually add remaining services
        services.AddScoped<IDatabaseConnectionProvider, SqLiteConnectionProvider>();
        services.AddScoped<IDatabaseInitializer, SqLiteDatabaseInitializer>();

        return services;
    }
}