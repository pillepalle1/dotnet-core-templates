namespace Pillepalle1.StatefulApi.Application.Model;

public class Problem
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required ProblemType ProblemType { get; set; }
    public required IEnumerable<string> Details { get; set; }


    public static Problem RequestValidationFailed(IEnumerable<string> details) => new Problem()
    {
        Title = "Request could not be validated",
        Description = "The provided request object was rejected by the model because one or more properties were out of range.",
        ProblemType = ProblemType.Validation,
        Details = details
    };
    public static Problem RequestValidationFailed() => RequestValidationFailed(Enumerable.Empty<string>());

    public static Problem ModelExceptionCaught(IEnumerable<string> details) => new Problem()
    {
        Title = "Model returned unsuccessfully",
        Description = "The request failed because the underlying model crashed during execution. Please see the logs for further details.",
        ProblemType = ProblemType.Crash,
        Details = details
    };
    public static Problem ModelExceptionCaught(Exception exception) => ModelExceptionCaught(exception.Message.ToEnumerable());
    public static Problem ModelExceptionCaught() => ModelExceptionCaught(Enumerable.Empty<string>());
    
    public static Problem EntityNotFound<TEntity>(string key) => new Problem()
    {
        Title = "Requested entity not found",
        Description = $"The requested entity of type {typeof(TEntity)} with key {key} could not be found",
        ProblemType = ProblemType.EntityNotFound,
        Details = $"Key = {key}".ToEnumerable()
    };

    public static Problem EntityExists<TEntity>(string key) => new Problem()
    {
        Title = "Entity already exists",
        Description = $"A create operation could not be executed, because the entity of type {typeof(TEntity)} with key {key} already exists",
        ProblemType = ProblemType.EntityExists,
        Details = $"Key = {key}".ToEnumerable()
    };
}

public enum ProblemType
{
    /// <summary>
    /// The core problem is, that a resource was not found
    /// </summary>
    EntityNotFound,
    
    /// <summary>
    /// The core problem is, that something, that is not expected to be there, already exists
    /// </summary>
    EntityExists,
    
    /// <summary>
    /// Indicates that something did not pass a validation
    /// </summary>
    Validation,
    
    /// <summary>
    /// Indicates that something crashed 
    /// </summary>
    Crash
}

internal static class ProblemExtensions
{
    public static IEnumerable<string> ToEnumerable(this string s) => Enumerable.Empty<string>().Append(s);
}