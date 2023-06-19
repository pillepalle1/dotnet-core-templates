using Microsoft.AspNetCore.Mvc;

namespace Pillepalle1.StatefulApi.WebApi.Extensions;
public static class CommonProblems
{
    public static IResult ToProblemDetailsResult(this Problem problem) => Results.Problem(new ProblemDetails()
    {
        Status = 500,
        Title = problem.Title,
        Detail = problem.Details.Aggregate(problem.Description, (old, patch) => $"{old}\n{patch}")
    });
    
    public static ProblemDetails JwtParametersMissing(string parameter) => new ProblemDetails()
    {
        Status = 500,
        Title = "JWT Parameters missing",
        Detail = $"Cannot create JWT Bearer Token because parameter {parameter} is missing"
    };
}