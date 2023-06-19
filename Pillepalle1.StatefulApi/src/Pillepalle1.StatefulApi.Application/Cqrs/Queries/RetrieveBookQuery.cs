

namespace Pillepalle1.StatefulApi.Application.Cqrs.Queries;

public class RetrieveBookQuery : ARequestBase<OneOf<Book,Problem>>
{
    public required string Isbn { init; get; }
}

public class RetrieveBookQueryValidator : AbstractValidator<RetrieveBookQuery>
{
    public RetrieveBookQueryValidator()
    {
        RuleFor(x => x)
            .NotNull().WithMessage("Query cannot be null");
    }
}

internal class RetrieveBookQueryHandler : ARequestHandlerBase<RetrieveBookQuery, Book>
{
    private readonly IDatabaseConnectionProvider _databaseConnectionProvider;

    public RetrieveBookQueryHandler(
        ILogger<RetrieveBookQueryHandler> logger,
        IEnumerable<IValidator<RetrieveBookQuery>> validators,
        IDatabaseConnectionProvider databaseConnectionProvider)
        : base(logger, validators)
    {
        _databaseConnectionProvider = databaseConnectionProvider;
    }

    public override async Task<OneOf<Book, Problem>> HandleImpl(RetrieveBookQuery request, CancellationToken cancellationToken)
    {
        var dbConnection = await _databaseConnectionProvider.ProvideAsync();

        var books = await dbConnection
            .QueryAsync<Book>(@"SELECT * FROM books WHERE isbn=@Isbn LIMIT 1;", request);
        books = books.ToList();

        return books.Any()
            ? books.First()
            : Problem.EntityNotFound<Book>(request.Isbn);
    }
}