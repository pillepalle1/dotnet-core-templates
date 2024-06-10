using Microsoft.Extensions.DependencyInjection;

namespace Customers.Application.Services.Database.SqLite;

public static class _Configuration
{
    public static IServiceCollection AddSqlite(this IServiceCollection services)
    {
        SqlMapper.AddTypeHandler(new GuidHandler());
        SqlMapper.AddTypeHandler(new DateTimeOffsetHandler());
        
        // Manually add remaining services
        services.AddSingleton<IDatabaseConnectionProvider, SqLiteConnectionProvider>();
        services.AddSingleton<IDatabaseInitializer, SqLiteDatabaseInitializer>();
        
        return services;
    }


    private class GuidHandler : SqlMapper.TypeHandler<Guid>
    {
        public override void SetValue(IDbDataParameter parameter, Guid guid)
        {
            parameter.Value = guid.ToString();
        }

        public override Guid Parse(object? value)
        {
            var guidValue = value?.ToString();
            if (guidValue is null)
            {
                return Guid.Empty;
            }

            return Guid.Parse(guidValue);
        }
    }

    private class DateTimeOffsetHandler : SqlMapper.TypeHandler<DateTimeOffset>
    {
        public override void SetValue(IDbDataParameter parameter, DateTimeOffset dateTimeOffset)
        {
            parameter.Value = dateTimeOffset.ToString();
        }

        public override DateTimeOffset Parse(object? value)
        {
            var dtoValue = value?.ToString();
            if (dtoValue is null)
            {
                return DateTimeOffset.MinValue;
            }

            return DateTimeOffset.Parse(dtoValue);
        }
    }
}