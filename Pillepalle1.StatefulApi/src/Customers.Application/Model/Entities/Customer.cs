namespace Customers.Application.Model.Entities;

public class Customer
{
    public required Guid Id { init; get; }
    public required string Name { init; get; }
    public required string Details { init; get; }
}

public class CustomerValidator : AbstractValidator<Customer>
{
    public CustomerValidator()
    {
        RuleFor(x => x.Id).IsValidId();
        RuleFor(x => x.Name).IsValidCustomerName();
        RuleFor(x => x.Details).AreValidCustomerDetails();
    }
}

public static class CustomerMappingExtensions
{
    public static Customer MapToEntity(this CreateCustomerCmd cmd) => new()
    {
        Id = Guid.NewGuid(),
        Name = cmd.Name,
        Details = cmd.Details
    };

    public static Customer MapToEntity(this UpdateCustomerCmd cmd) => new()
    {
        Id = cmd.CustomerId,
        Name = cmd.Name,
        Details = cmd.Details
    };
}

public static class CustomerNormalizingExtensions
{
    public static string NormalizeCustomerName(this string customerName)
        => customerName.Trim().ToUpper();
}

public static class CustomerValidationExtensions
{
    public static IRuleBuilderOptions<T, string> IsValidCustomerName<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder.MinimumLength(3).WithMessage("Name must consist of at least three characters");
    
    public static IRuleBuilderOptions<T, string> AreValidCustomerDetails<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder.NotEmpty().WithMessage("Please try to provide at least some information about the customer");
}
