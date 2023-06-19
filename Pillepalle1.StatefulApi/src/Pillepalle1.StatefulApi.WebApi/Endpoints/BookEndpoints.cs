namespace Pillepalle1.StatefulApi.WebApi.Endpoints;

internal static class BookEndpoints
{
    internal static void RegisterBookEndpoints(this WebApplication app)
    {
        app.MapPut("/books", PUT_Book)
            .RequireAuthorization("can-modify-books")
            .Accepts<CreateBookRequest>("application/json")
            .Produces(204)
            .Produces(400)
            .Produces<ProblemDetails>(500)
            .WithTags("Books");

        app.MapGet("/books", GET_Books)
            .Produces<IEnumerable<BookResponse>>()
            .Produces<ProblemDetails>(500)
            .WithTags("Books");

        app.MapGet("/books/{isbn}", GET_Book)
            .Produces<BookResponse>()
            .Produces<ProblemDetails>(500)
            .WithTags("Books");

        app.MapPost("/books/{isbn}", POST_Book)
            .RequireAuthorization("can-modify-books")
            .Produces<int>()
            .Produces<ProblemDetails>(500)
            .WithTags("Books");

        app.MapDelete("/books/{isbn}", DELETE_Book)
            .RequireAuthorization("can-modify-books")
            .Produces<int>(204)
            .Produces(404)
            .WithTags("Books");
    }

    private static async Task<IResult> PUT_Book(
        CreateBookRequest request,
        IMediator mediator)
    {
        var createBook = await mediator.Send(request.ToCqrs());
        if (createBook.Failed()) return createBook.Problem().ToProblemDetailsResult();

        var rowsAffected = createBook.Unwrap();
        return rowsAffected > 0
            ? Results.Created($"/books/{request.Isbn}", request)
            : Results.BadRequest();
    }

    private static async Task<IResult> GET_Books(
        IMediator mediator)
    {
        var fetchAllBooks = await mediator.Send(new RetrieveAllBooksQuery());
        if (fetchAllBooks.Failed())
            return fetchAllBooks.Problem().ToProblemDetailsResult();

        return Results.Ok(fetchAllBooks.Unwrap().Select(x => x.ToResponse()));
    }

    private static async Task<IResult> GET_Book(
        string isbn,
        IMediator mediator)
    {
        var fetchBook = await mediator.Send(new RetrieveBookQuery()
        {
            Isbn = isbn
        });

        return fetchBook.Succeeded()
            ? Results.Ok(fetchBook.Unwrap().ToResponse())
            : fetchBook.Problem().ToProblemDetailsResult();
    }

    private static async Task<IResult> POST_Book(
        UpdateBookRequest request,
        string isbn,
        IMediator mediator)
    {
        var updateBook = await mediator.Send(request.ToCqrs(isbn));
        if (updateBook.Failed())
            return updateBook.Problem().ToProblemDetailsResult();

        var rowsAffected = updateBook.Unwrap();
        return rowsAffected > 0
            ? Results.Ok(rowsAffected)
            : Results.NotFound();
    }

    private static async Task<IResult> DELETE_Book(
        string isbn,
        IMediator mediator)
    {
        var deleteBook = await mediator.Send(new DeleteBookCmd()
        {
            Isbn = isbn
        });
        if (deleteBook.Failed())
            return deleteBook.Problem().ToProblemDetailsResult();

        var rowsAffected = deleteBook.Unwrap();
        return rowsAffected > 0
            ? Results.NoContent()
            : Results.NotFound();
    }
}