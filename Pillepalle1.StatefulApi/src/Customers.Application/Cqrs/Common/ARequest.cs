using System.Diagnostics;

namespace Customers.Application.Cqrs.Common;

public abstract class ARequest<TResponse> : IRequest<OneOf<TResponse,Problem>>
{
    internal Guid MediatorRequestId { init; get; } = Guid.NewGuid();
    public Guid GetRequestId() => MediatorRequestId;

    internal Stopwatch Stopwatch { init; get; } = new Stopwatch();
    public TimeSpan GetElapsedTime() => Stopwatch.Elapsed;
}