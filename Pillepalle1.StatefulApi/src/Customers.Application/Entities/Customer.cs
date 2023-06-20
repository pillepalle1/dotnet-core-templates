namespace Customers.Application.Entities;

public class Customer
{
    public required long Id { init; get; }
    public required string Name { init; get; }
    public required string Details { init; get; }
}