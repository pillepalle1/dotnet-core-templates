namespace Customers.Application.Extensions;

internal static class NormalizingExtensions
{
    public static string DefaultNormalization(this string s)
        => s.Trim().ToUpper();

    public static string LightNormalization(this string s)
        => s.Trim();

}