namespace Pillepalle1.StatefulApi.Application.Cqrs.Commands;

public class UpdateBookCmd : ARequestBase<OneOf<int, Problem>>
{
    public required string Isbn { init; get; }
    public string? Title { init; get; }
}

public class UpdateBookCmdValidator : AbstractValidator<UpdateBookCmd>
{
    public UpdateBookCmdValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title cannot be empty")
            .When(x => x.Title is not null);
    }
}

internal class UpdateBookCmdHandler : ARequestHandlerBase<UpdateBookCmd, int>
{
    private readonly IDatabaseConnectionProvider _databaseConnectionProvider;

    public UpdateBookCmdHandler(
        ILogger<UpdateBookCmdHandler> logger,
        IEnumerable<IValidator<UpdateBookCmd>> validators,
        IDatabaseConnectionProvider databaseConnectionProvider)
        : base(logger, validators)
    {
        _databaseConnectionProvider = databaseConnectionProvider;
    }

    public override async Task<OneOf<int, Problem>> HandleImpl(UpdateBookCmd request, CancellationToken cancellationToken)
    {
        var dbConnection = await _databaseConnectionProvider.ProvideAsync();
        var rowsAffected = await dbConnection.ExecuteAsync(@"UPDATE books SET title=@Title WHERE isbn=@Isbn;", request);
        
        return rowsAffected > 0
            ? rowsAffected
            : Problem.EntityNotFound<Book>(request.Isbn);
    }
}