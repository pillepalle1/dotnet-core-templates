namespace Customers.Application.Cqrs.Commands;

public class CreateCustomerCmd : ARequestBase<OneOf<long, Problem>>
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

internal class CreateCustomerCmdHandler : ARequestHandlerBase<CreateCustomerCmd, long>
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

    public override async Task<OneOf<long, Problem>> HandleImpl(CreateCustomerCmd request, CancellationToken cancellationToken)
    {
        var dbConnection = await _databaseConnectionProvider.ProvideAsync();

        var rng = new Random();
        var id = rng.Next();
        
        var queryParams = new
        {
            Id = id,
            request.Name,
            request.Details
        };
        
        var rowsAffected = await dbConnection.ExecuteAsync(
            @"INSERT INTO customers (Id,Name,Details) VALUES (@Id,@Name,@Details) ON CONFLICT DO NOTHING;", queryParams);

        return rowsAffected > 0
            ? id
            : Problem.EntityExists<Customer>(request.Name);
    }
}