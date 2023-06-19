namespace Pillepalle1.StatefulApi.Application.Cqrs.Commands;

public class CreateBookCmd : ARequestBase<OneOf<int, Problem>>
{
    public required string Isbn { init; get; }
    public required string Title { init; get; }
}

public class CreateBookCmdValidator : AbstractValidator<CreateBookCmd>
{
    public CreateBookCmdValidator()
    {
        RuleFor(x => x.Isbn)
            .NotNull().WithMessage("Isbn is required");
        
        RuleFor(x => x.Title)
            .NotNull().WithMessage("Title is required");
    }
}

internal class CreateBookCommandHandler : ARequestHandlerBase<CreateBookCmd, int>
{
    private readonly IDatabaseConnectionProvider _databaseConnectionProvider;

    public CreateBookCommandHandler(
        ILogger<CreateBookCommandHandler> logger,
        IEnumerable<IValidator<CreateBookCmd>> validators,
        IDatabaseConnectionProvider databaseConnectionProvider) 
        : base(logger, validators)
    {
        _databaseConnectionProvider = databaseConnectionProvider;
    }

    public override async Task<OneOf<int, Problem>> HandleImpl(CreateBookCmd request, CancellationToken cancellationToken)
    {
        var dbConnection = await _databaseConnectionProvider.ProvideAsync();
        var rowsAffected = await dbConnection.ExecuteAsync(
            @"INSERT INTO books (isbn,title) VALUES (@Isbn,@Title) ON CONFLICT DO NOTHING;", request);

        return rowsAffected > 0
            ? rowsAffected
            : Problem.EntityExists<Book>(request.Isbn);
    }
}