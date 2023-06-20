using Microsoft.AspNetCore.Mvc;

namespace Customers.WebApi.Extensions;
public static class CommonProblems
{
    public static IResult ToProblemDetailsResult(this Problem problem) => Results.Problem(new ProblemDetails()
    {
        Status = problem.StatusCode(),
        Title = problem.Title,
        Detail = problem.Details.Aggregate(problem.Description, (old, patch) => $"{old}\n{patch}")
    });
    
    private static int StatusCode(this Problem problem) => problem.ProblemType switch
    {
        ProblemType.Crash => 500,
        ProblemType.Validation => 500,
        ProblemType.EntityExists => 400,
        ProblemType.EntityNotFound => 404,
        _ => 500
    };
}