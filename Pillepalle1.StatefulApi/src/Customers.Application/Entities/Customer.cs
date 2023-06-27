namespace Customers.Application.Entities;

public class Customer
{
    public required Guid Id { init; get; }
    public required string Name { init; get; }
    public required string Details { init; get; }
}

internal static class CustomerDatabaseExtensions
{
    public static async Task<bool> ExistsCustomerEntryAsync(this IDbConnection dbConnection, Guid customerId)
    {
        var sql = @"SELECT COUNT(*) FROM Customers WHERE Id=@Id;";

        var queryParams = new
        {
            Id = customerId
        };

        var count = await dbConnection.ExecuteScalarAsync<int>(sql, queryParams);
        return 0 != count;
    }
    
    public static async Task<bool> CreateCustomerEntryAsync(this IDbConnection dbConnection, Customer customer)
    {
        var sql = @"INSERT INTO Customers (Id,Name,Details) VALUES (@Id,@Name,@Details);";

        var rowsAffected = await dbConnection.ExecuteAsync(sql, customer);
        return rowsAffected > 0;
    }

    public static async Task<bool> UpdateCustomerEntryAsync(this IDbConnection dbConnection, Customer customer)
    {
        var sql = @"UPDATE Customers SET Name=@Name,Details=@Details WHERE Id=@Id;";
        var rowsAffected = await dbConnection.ExecuteAsync(sql, customer);
        return rowsAffected > 0;
    }

    public static async Task<IEnumerable<Customer>> RetrieveAllCustomerEntriesAsync(this IDbConnection dbConnection)
    {
        var sql = @"SELECT * FROM Customers;";
        return await dbConnection.QueryAsync<Customer>(sql);
    }

    public static async Task<bool> DeleteCustomerEntryAsync(this IDbConnection dbConnection, Guid customerId)
    {
        var sql = @"DELETE FROM Customers WHERE Id=@Id;";

        var queryParams = new
        {
            Id = customerId,
        };
        
        var rowsAffected = await dbConnection.ExecuteAsync(sql, queryParams);
        return rowsAffected > 0;
    }

    public static async Task<Customer> RetrieveCustomerEntryAsync(this IDbConnection dbConnection, Guid customerId)
    {
        var sql = @"SELECT * FROM Customers WHERE Id=@Id LIMIT 1;";

        var queryParams = new
        {
            Id = customerId
        };
        
        return await dbConnection.QuerySingleAsync<Customer>(sql, queryParams);
    }
}