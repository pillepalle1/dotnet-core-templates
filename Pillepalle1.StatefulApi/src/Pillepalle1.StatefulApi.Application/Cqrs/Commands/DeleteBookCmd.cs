namespace Pillepalle1.StatefulApi.Application.Cqrs.Commands;

public class DeleteBookCmd : ARequestBase<OneOf<int,Problem>>
{
    public required string Isbn { init; get; }
}

public class DeleteBookCmdValidator : AbstractValidator<DeleteBookCmd>
{
    public DeleteBookCmdValidator()
    {
        RuleFor(x => x.Isbn)
            .NotNull().WithMessage("Isbn cannot be null");
    }
}

internal class DeleteBookCmdHandler : ARequestHandlerBase<DeleteBookCmd, int>
{
    private readonly IDatabaseConnectionProvider _databaseConnectionProvider;

    public DeleteBookCmdHandler(
        ILogger<DeleteBookCmdHandler> logger,
        IEnumerable<IValidator<DeleteBookCmd>> validators,
        IDatabaseConnectionProvider databaseConnectionProvider)
        : base(logger, validators)
    {
        _databaseConnectionProvider = databaseConnectionProvider;
    }

    public override async Task<OneOf<int, Problem>> HandleImpl(DeleteBookCmd request, CancellationToken cancellationToken)
    {
        var dbConnection = await _databaseConnectionProvider.ProvideAsync();
        var rowsAffected = await dbConnection.ExecuteAsync(@"DELETE FROM books WHERE isbn=@Isbn;", request);
        
        return rowsAffected > 0
            ? rowsAffected
            : Problem.EntityNotFound<Book>(request.Isbn);
    }
}