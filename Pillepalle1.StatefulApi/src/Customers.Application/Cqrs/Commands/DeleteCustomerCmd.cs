namespace Customers.Application.Cqrs.Commands;

public class DeleteCustomerCmd : ARequestBase<OneOf<int,Problem>>
{
    public required Guid Id { init; get; }
}

public class DeleteCustomerCmdValidator : AbstractValidator<DeleteCustomerCmd>
{
    public DeleteCustomerCmdValidator()
    { 
        RuleFor(x => x.Id).IsValidId();
    }
}

internal class DeleteCustomerCmdHandler : ARequestHandlerBase<DeleteCustomerCmd, int>
{
    private readonly IDatabaseConnectionProvider _databaseConnectionProvider;

    public DeleteCustomerCmdHandler(
        ILogger<DeleteCustomerCmdHandler> logger,
        IEnumerable<IValidator<DeleteCustomerCmd>> validators,
        IDatabaseConnectionProvider databaseConnectionProvider)
        : base(logger, validators)
    {
        _databaseConnectionProvider = databaseConnectionProvider;
    }

    public override async Task<OneOf<int, Problem>> HandleImpl(DeleteCustomerCmd request, CancellationToken cancellationToken)
    {
        var dbConnection = await _databaseConnectionProvider.ProvideAsync();

        var sql = @"DELETE FROM customers WHERE Id=@Id;";
        var rowsAffected = await dbConnection.ExecuteAsync(sql, request);
        
        return rowsAffected > 0
            ? rowsAffected
            : Problem.EntityNotFound<Customer>(request.Id.ToString());
    }
}