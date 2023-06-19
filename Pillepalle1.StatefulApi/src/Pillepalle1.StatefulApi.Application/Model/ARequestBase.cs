using System.Diagnostics;

namespace Pillepalle1.StatefulApi.Application.Model;
public abstract class ARequestBase<TResponse> : IRequest<TResponse>
{
    internal Guid MediatorRequestId { init; get; } = Guid.NewGuid();
    public Guid GetRequestId() => MediatorRequestId;

    internal Stopwatch Stopwatch { init; get; } = new Stopwatch();
    public TimeSpan GetElapsedTime() => Stopwatch.Elapsed;
}