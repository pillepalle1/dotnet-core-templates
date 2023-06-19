namespace Pillepalle1.StatefulApi.Application.Model;

public static class OneOfExtensions
{
    public static bool Succeeded<T>(this OneOf<T, Problem> result)
        => result.IsT0;

    public static bool Failed<T>(this OneOf<T, Problem> result)
        => result.IsT1;

    public static T Unwrap<T>(this OneOf<T, Problem> result)
        => result.AsT0;

    public static Problem Problem<T>(this OneOf<T, Problem> result)
        => result.AsT1;
}