namespace Customers.Application.Extensions;

internal static class ValidationExtensions
{
    public static IRuleBuilderOptions<T, Guid> IsValidId<T>(this IRuleBuilder<T, Guid> ruleBuilder) =>
        ruleBuilder.Must(x => !x.Equals(Guid.Empty)).WithMessage("Empty Guid is not a valid Id");
    
    public static IRuleBuilderOptions<T, long> IsValidId<T>(this IRuleBuilder<T, long> ruleBuilder) =>
        ruleBuilder.GreaterThanOrEqualTo(0).WithMessage("Ids cannot be negative");
    
    public static IRuleBuilderOptions<T, int> IsValidId<T>(this IRuleBuilder<T, int> ruleBuilder) =>
        ruleBuilder.GreaterThanOrEqualTo(0).WithMessage("Ids cannot be negative");
    public static IRuleBuilderOptions<T, string> IsValidCustomerName<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder.MinimumLength(3).WithMessage("Name must consist of at least three characters");
    public static IRuleBuilderOptions<T, string> AreValidCustomerDetails<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder.NotEmpty().WithMessage("Please try to provide at least some information about the customer");
}