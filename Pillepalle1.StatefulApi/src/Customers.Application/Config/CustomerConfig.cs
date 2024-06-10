namespace Customers.Application.Config;

public sealed class CustomerConfig
{
    public const string SectionName = "Customers";
    
    public string Name { set; get; } = String.Empty;
}