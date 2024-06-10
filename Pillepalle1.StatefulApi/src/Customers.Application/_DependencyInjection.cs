using System.Reflection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Customers.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration config)
    {
        // Automagically add services via assembly scanning
        var executingAssembly = Assembly.GetExecutingAssembly();
        services.AddValidatorsFromAssembly(executingAssembly);
        services.AddMediatR(executingAssembly);
        
        // Manually add remaining services
        services.AddConfiguration(config);
        
        services.AddSqlite();
        services.AddRepositories();

        return services;
    }

    private static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<CustomerConfig>(config.GetSection(CustomerConfig.SectionName));
        services.AddTransient<CustomerConfig>(provider => provider.GetRequiredService<IOptions<CustomerConfig>>().Value);

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton<ICustomerRepository, SqliteCustomerRepository>();
        
        return services;
    }
}
