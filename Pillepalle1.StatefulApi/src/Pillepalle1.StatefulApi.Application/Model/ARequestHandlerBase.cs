using System.Text.Json;

namespace Pillepalle1.StatefulApi.Application.Model;

public abstract class ARequestHandlerBase<TRequest,TResponse> : IRequestHandler<TRequest,OneOf<TResponse, Problem>>
    where TRequest : ARequestBase<OneOf<TResponse,Problem>>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ILogger<ARequestHandlerBase<TRequest, TResponse>> _logger;

    public ARequestHandlerBase(
        ILogger<ARequestHandlerBase<TRequest,TResponse>> logger,
        IEnumerable<IValidator<TRequest>> validators)
    {
        _logger = logger;
        _validators = validators;
    }

    public abstract Task<OneOf<TResponse, Problem>> HandleImpl(TRequest request, CancellationToken cancellationToken);
    
    public async Task<OneOf<TResponse, Problem>> Handle(TRequest request, CancellationToken cancellationToken)
    {
        // TIME THE OPERATION
        request.Stopwatch.Restart();
        
        // ------------------------------------------------------------------------------------------------------------
        // PERFORM THE ACTUAL OPERATION
        var response = await GenerateResponseAsync(request, cancellationToken);
        await PostProcessResponseAsync(request, response, cancellationToken);
        // ------------------------------------------------------------------------------------------------------------
        
        // END TIMING
        request.Stopwatch.Stop();
        
        return response;
    }

    private async Task<OneOf<TResponse, Problem>> GenerateResponseAsync(TRequest request, CancellationToken cancellationToken)
    {
        // LOG INCOMING REQUESTS
        // This allows for tracing what happened in the past
        _logger.LogInformation("[Processing: {RequestId}] {Request}", request.MediatorRequestId, JsonSerializer.Serialize(request));
        
        // VALIDATE INCOMING REQUESTS
        // I am deeply agitated that this cannot be achieved through an IPipelineBehaviour, but apparently it is not
        // possible to use OneOf in IPipelineHandlers to return a Problem Instance there
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(_validators.Select(x => x.ValidateAsync(context, cancellationToken)));
            var failures = validationResults.Where(r => r.Errors.Any()).SelectMany(x => x.Errors).ToList();

            if (failures.Any()) 
                return Problem.RequestValidationFailed(failures.Select(x => x.ErrorMessage));
        }

        // WRAP LOGIC INTO TRY-CATCH
        // We do not want to expose Exceptions to the consumer. If an exception is thrown, it is converted to a Problem instance
        // and the user is informed about the failed request that way
        try
        {
            return await HandleImpl(request, cancellationToken);
        }
        catch (Exception e)
        {
#if DEBUG
            throw;      
#endif
            return Problem.ModelExceptionCaught(e);
        }
    }
    private async Task PostProcessResponseAsync(
        TRequest request,
        OneOf<TResponse, Problem> response,
        CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        
        // LOG REASONS FOR FAILED REQUESTS
        if (response.Failed())
        {
            var problem = response.Problem();
            _logger.LogWarning(
                "Failed: {RequestId} | {Details}", 
                request.MediatorRequestId, 
                problem.Details.Aggregate(problem.ProblemType.ToString(), (prev, next) => $"{prev}; {next}"));
        }
    }
}