namespace Customers.WebApi.Endpoints;

internal static class CustomerEndpoints
{
    internal static void RegisterCustomerEndpoints(this WebApplication app)
    {
        app.MapPut("/customers", PUT_Customer)
            .Accepts<CreateCustomerRequest>("application/json")
            .Produces<CustomerResponse>(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(400)
            .Produces<ProblemDetails>(500)
            //.RequireAuthorization("can-modify-customers")
            .WithTags("Customers");

        app.MapGet("/customers", GET_Customers)
            .Produces<IEnumerable<CustomerResponse>>()
            .Produces<ProblemDetails>(500)
            //.RequireAuthorization("can-view-customers")
            .WithTags("Customers");

        app.MapGet("/customers/{id:guid}", GET_Customer)
            .Produces<CustomerResponse>()
            .Produces<ProblemDetails>(404)
            .Produces<ProblemDetails>(500)
            //.RequireAuthorization("can-view-customers")
            .WithTags("Customers");

        app.MapPost("/customers/{id:guid}", POST_Customer)
            .Produces(200)
            .Produces<ProblemDetails>(400)
            .Produces<ProblemDetails>(404)
            .Produces<ProblemDetails>(500)
            //.RequireAuthorization("can-modify-customers")
            .WithTags("Customers");

        app.MapDelete("/customers/{id:guid}", DELETE_Customer)
            .Produces(204)
            .Produces<ProblemDetails>(404)
            .Produces<ProblemDetails>(500)
            //.RequireAuthorization("can-modify-customers")
            .WithTags("Customers");
    }

    private static async Task<IResult> PUT_Customer(
        CreateCustomerRequest request,
        IMediator mediator)
    {
        var create = await mediator.Send(request.ToCqrs());
        return create.Succeeded()
            ? Results.Created($"/customers/{create.Unwrap().Id}", create.Unwrap())
            : create.Problem().ToProblemDetailsResult();
    }

    private static async Task<IResult> GET_Customers(
        IMediator mediator)
    {
        var query = new AllCustomersQuery();
        var fetchAll = await mediator.Send(query);
        return fetchAll.Succeeded()
            ? Results.Ok(fetchAll.Unwrap().Select(x => x.ToResponse()))
            : fetchAll.Problem().ToProblemDetailsResult();
    }

    private static async Task<IResult> GET_Customer(
        Guid id,
        IMediator mediator)
    {
        var query = new CustomerQuery()
        {
            CustomerId = id
        };
        
        var fetch = await mediator.Send(query);

        return fetch.Succeeded()
            ? Results.Ok(fetch.Unwrap().ToResponse())
            : fetch.Problem().ToProblemDetailsResult();
    }

    private static async Task<IResult> POST_Customer(
        UpdateCustomerRequest request,
        Guid id,
        IMediator mediator)
    {
        // Fetch the existing entity
        var fetch = await mediator.Send(new CustomerQuery()
        {
            CustomerId = id
        });
        if (!fetch.Succeeded()) return fetch.Problem().ToProblemDetailsResult();
        
        // Update
        var update = await mediator.Send(request.ToCqrs(fetch.Unwrap()));
        return update.Succeeded()
            ? Results.Ok(update.Unwrap().ToResponse())
            : update.Problem().ToProblemDetailsResult();
    }

    private static async Task<IResult> DELETE_Customer(
        Guid id,
        IMediator mediator)
    {
        var cmd = new DeleteCustomerCmd()
        {
            CustomerId = id
        };
        
        var delete = await mediator.Send(cmd);
        return delete.Succeeded()
            ? Results.NoContent()
            : delete.Problem().ToProblemDetailsResult();
    }
}