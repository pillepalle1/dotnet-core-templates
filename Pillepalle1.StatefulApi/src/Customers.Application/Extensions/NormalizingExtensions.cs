namespace Customers.Application.Extensions;

public static class NormalizingExtensions
{
    public static string NormalizeCurrency(this string currency) 
        => currency.Trim().ToUpper();

    public static string NormalizePlatform(this string platform)
        => platform.Trim().ToUpper();

    public static string NormalizePlatformAccountId(this string account)
        => account.Trim();

    public static string NormalizeClubTag(this string tag)
        => tag.Trim().ToUpper();

    public static string NormalizeScreenName(this string screenName)
        => screenName.Trim();
}