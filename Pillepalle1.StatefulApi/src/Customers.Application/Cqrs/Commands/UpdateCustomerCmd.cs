namespace Customers.Application.Cqrs.Commands;

public class UpdateCustomerCmd : ARequestBase<OneOf<int, Problem>>
{
    public required Guid Id { init; get; }
    public required string Name { init; get; }
    public required string Details { init; get; }
}

public class UpdateCustomerCmdValidator : AbstractValidator<UpdateCustomerCmd>
{
    public UpdateCustomerCmdValidator()
    {
        RuleFor(x => x.Id).IsValidId();
        RuleFor(x => x.Name).IsValidCustomerName();
        RuleFor(x => x.Details).AreValidCustomerDetails();
    }
}

internal class UpdateCustomerCmdHandler : ARequestHandlerBase<UpdateCustomerCmd, int>
{
    private readonly IDatabaseConnectionProvider _databaseConnectionProvider;

    public UpdateCustomerCmdHandler(
        ILogger<UpdateCustomerCmdHandler> logger,
        IEnumerable<IValidator<UpdateCustomerCmd>> validators,
        IDatabaseConnectionProvider databaseConnectionProvider)
        : base(logger, validators)
    {
        _databaseConnectionProvider = databaseConnectionProvider;
    }

    public override async Task<OneOf<int, Problem>> HandleImpl(UpdateCustomerCmd request, CancellationToken cancellationToken)
    {
        var dbConnection = await _databaseConnectionProvider.ProvideAsync();

        var sql = @"UPDATE customers SET Name=@Name,Details=@Details WHERE Id=@Id;";
        var rowsAffected = await dbConnection.ExecuteAsync(sql, request);
        
        return rowsAffected > 0
            ? rowsAffected
            : Problem.EntityNotFound<Customer>(request.Id.ToString());
    }
}