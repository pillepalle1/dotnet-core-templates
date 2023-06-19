namespace Pillepalle1.StatefulApi.Application.Cqrs.Queries;

public class RetrieveAllBooksQuery : ARequestBase<OneOf<ImmutableList<Book>, Problem>>
{

}

internal class RetrieveAllBooksQueryHandler : ARequestHandlerBase<RetrieveAllBooksQuery, ImmutableList<Book>>
{
    private readonly IDatabaseConnectionProvider _databaseConnectionProvider;

    public RetrieveAllBooksQueryHandler(
        ILogger<RetrieveAllBooksQueryHandler> logger,
        IEnumerable<IValidator<RetrieveAllBooksQuery>> validators,
        IDatabaseConnectionProvider databaseConnectionProvider) 
        : base(logger, validators)
    {
        _databaseConnectionProvider = databaseConnectionProvider;
    }

    public override async Task<OneOf<ImmutableList<Book>, Problem>> HandleImpl(RetrieveAllBooksQuery request, CancellationToken cancellationToken)
    {
        var dbConnection = await _databaseConnectionProvider.ProvideAsync();

        var books = await dbConnection.QueryAsync<Book>(@"SELECT * FROM books;");
        return books.ToImmutableList();
    }
}