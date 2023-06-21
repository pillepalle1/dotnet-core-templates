using System.Reflection;
using Customers.Application.Services.SqLite;
using Microsoft.Extensions.DependencyInjection;

namespace Customers.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        SqlMapper.AddTypeHandler(new GuidHandler());
        SqlMapper.AddTypeHandler(new TimeSpanHandler());
        SqlMapper.AddTypeHandler(new DateTimeOffsetHandler());
        
        // Automagically add services via assembly scanning
        var executingAssembly = Assembly.GetExecutingAssembly();
        services.AddValidatorsFromAssembly(executingAssembly);
        services.AddMediatR(executingAssembly);

        // Manually add remaining services
        services.AddScoped<IDatabaseConnectionProvider, SqLiteConnectionProvider>();
        services.AddScoped<IDatabaseInitializer, SqLiteDatabaseInitializer>();

        return services;
    }

    internal abstract class SqliteTypeHandler<T> : SqlMapper.TypeHandler<T>
    {
        public override void SetValue(IDbDataParameter parameter, T value) => parameter.Value = value;
    }
    
    internal class GuidHandler : SqliteTypeHandler<Guid>
    {
        public override Guid Parse(object value) => Guid.Parse((string) value);
    }

    internal class DateTimeOffsetHandler : SqliteTypeHandler<DateTimeOffset>
    {
        public override DateTimeOffset Parse(object value) => DateTimeOffset.Parse((string) value);
    }
    
    internal class TimeSpanHandler : SqliteTypeHandler<TimeSpan>
    {
        public override TimeSpan Parse(object value) => TimeSpan.Parse((string) value);
    }
}