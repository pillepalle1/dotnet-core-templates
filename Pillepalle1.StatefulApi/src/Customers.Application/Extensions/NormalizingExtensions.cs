namespace Customers.Application.Extensions;

public static class NormalizingExtensions
{
    public static string NormalizeCurrency(this string currency) 
        => currency.Trim().ToUpper();
}