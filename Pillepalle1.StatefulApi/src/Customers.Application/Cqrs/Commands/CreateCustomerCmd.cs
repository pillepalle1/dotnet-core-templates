namespace Customers.Application.Cqrs.Commands;

public class CreateCustomerCmd : ARequestBase<OneOf<Guid, Problem>>
{
    public required string Name { init; get; }
    public required string Details { init; get; }
}

public class CreateCustomerCmdValidator : AbstractValidator<CreateCustomerCmd>
{
    public CreateCustomerCmdValidator()
    {
        RuleFor(x => x.Name).IsValidCustomerName();
        RuleFor(x => x.Details).AreValidCustomerDetails();
    }
}

internal class CreateCustomerCmdHandler : ARequestHandlerBase<CreateCustomerCmd, Guid>
{
    private readonly IDatabaseConnectionProvider _databaseConnectionProvider;

    public CreateCustomerCmdHandler(
        ILogger<CreateCustomerCmdHandler> logger,
        IEnumerable<IValidator<CreateCustomerCmd>> validators,
        IDatabaseConnectionProvider databaseConnectionProvider) 
        : base(logger, validators)
    {
        _databaseConnectionProvider = databaseConnectionProvider;
    }

    public override async Task<OneOf<Guid, Problem>> HandleImpl(CreateCustomerCmd request, CancellationToken cancellationToken)
    {
        var dbConnection = await _databaseConnectionProvider.ProvideAsync();

        var id = Guid.NewGuid();
        var queryParams = new
        {
            Id = id,
            request.Name,
            request.Details
        };

        var sql = @"INSERT INTO customers (Id,Name,Details) VALUES (@Id,@Name,@Details) ON CONFLICT DO NOTHING;"; 
        var rowsAffected = await dbConnection.ExecuteAsync(sql, queryParams);

        return rowsAffected > 0
            ? id
            : Problem.EntityExists<Customer>(request.Name);
    }
}