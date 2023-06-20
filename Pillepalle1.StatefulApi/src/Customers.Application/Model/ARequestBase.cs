using System.Diagnostics;

namespace Customers.Application.Model;
public abstract class ARequestBase<TResponse> : IRequest<TResponse>
{
    internal Guid MediatorRequestId { init; get; } = Guid.NewGuid();
    public Guid GetRequestId() => MediatorRequestId;

    internal Stopwatch Stopwatch { init; get; } = new Stopwatch();
    public TimeSpan GetElapsedTime() => Stopwatch.Elapsed;
}