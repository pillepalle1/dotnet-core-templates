namespace Customers.Application.Services.Database.Repositories;

internal interface ICustomerRepository
{
    // Create
    
    Task<Customer?> CreateAsync(Customer customer, IDbConnection dbConnection, IDbTransaction? dbTransaction = null);

    // Read
    
    Task<Customer?> ReadByIdAsync(Guid customerId, IDbConnection dbConnection, IDbTransaction? dbTransaction = null);

    Task<IEnumerable<Customer>> ReadPagedAsync(
        IDbConnection dbConnection,
        int? pageSize = null,
        Guid? prevGuid = null,
        IDbTransaction? dbTransaction = null);

    Task<IEnumerable<Customer>> ReadAllAsync(IDbConnection dbConnection, IDbTransaction? dbTransaction = null);

    // Update
    
    Task<Customer?> UpdateAsync(Customer customer, IDbConnection dbConnection, IDbTransaction? dbTransaction = null);
    
    Task<Customer?> UpdateOrCreateAsync(Customer customer, IDbConnection dbConnection, IDbTransaction? dbTransaction = null);

    // Delete
    
    Task<int> DeleteAsync(Guid customerId, IDbConnection dbConnection, IDbTransaction? dbTransaction = null);
}

internal class SqliteCustomerRepository : ICustomerRepository
{
    // Create
    
    public async Task<Customer?> CreateAsync(Customer customer, IDbConnection dbConnection, IDbTransaction? dbTransaction = null)
    {
        var rowsAffectedByCreate = await dbConnection.ExecuteAsync(CreateCustomerSql, customer, dbTransaction);
        if (rowsAffectedByCreate > 0)
        {
            return customer;
        }

        return null;
    }
    
    // Read

    public async Task<Customer?> ReadByIdAsync(Guid customerId, IDbConnection dbConnection, IDbTransaction? dbTransaction = null)
    {
        var queryParams = new
        {
            Id = customerId
        };
        
        return await dbConnection.QuerySingleOrDefaultAsync<Customer>(ReadCustomerByIdSql, queryParams, dbTransaction);
    }

    private const string ReadCustomerByIdSql =
        """
        SELECT * FROM Customers WHERE Id=@Id LIMIT 1;
        """;

    public async Task<IEnumerable<Customer>> ReadPagedAsync(
        IDbConnection dbConnection,
        int? pageSize = null,
        Guid? prevLargestGuid = null,
        IDbTransaction? dbTransaction = null)
    {
        var queryParams = new
        {
            PrevLargestGuid = prevLargestGuid ?? Guid.Empty,
            PageSize = pageSize ?? 50
        };

        return await dbConnection.QueryAsync<Customer>(ReadCustomersPagedSql, queryParams, dbTransaction);
    }

    private const string ReadCustomersPagedSql =
        """
        SELECT * FROM Customers WHERE Id > @PrevLargestGuid ORDER BY Id LIMIT @PageSize;
        """;
    
    public async Task<IEnumerable<Customer>> ReadAllAsync(IDbConnection dbConnection, IDbTransaction? dbTransaction) => 
        await dbConnection.QueryAsync<Customer>(ReadAllCustomersSql, null, dbTransaction);

    private const string ReadAllCustomersSql =
        """
        SELECT * FROM Customers;
        """;
    
    // Update
    
    public async Task<Customer?> UpdateAsync(Customer customer, IDbConnection dbConnection, IDbTransaction? dbTransaction = null)
    {
        var rowsAffectedByUpdate = await dbConnection.ExecuteAsync(UpdateCustomerSql, customer, dbTransaction);
        if (rowsAffectedByUpdate > 0)
        {
            return customer;
        }

        return null;
    }
    
    public async Task<Customer?> UpdateOrCreateAsync(Customer customer, IDbConnection dbConnection, IDbTransaction? dbTransaction = null)
    {
        var rowsAffectedByUpdate = await dbConnection.ExecuteAsync(UpdateCustomerSql, customer, dbTransaction);
        if (rowsAffectedByUpdate > 0)
        {
            return customer;
        }

        var rowsAffectedByCreate = await dbConnection.ExecuteAsync(CreateCustomerSql, customer, dbTransaction);
        if (rowsAffectedByCreate > 0)
        {
            return customer;
        }

        return null;

    }

    private const string UpdateCustomerSql =
        """
        UPDATE Customers SET Name=@Name,Details=@Details WHERE Id=@Id;
        """;
    
    private const string CreateCustomerSql = 
        """
        INSERT INTO Customers (Id,Name,Details) VALUES (@Id,@Name,@Details);
        """; 
    
    // Delete

    public async Task<int> DeleteAsync(Guid customerId, IDbConnection dbConnection, IDbTransaction? dbTransaction = null)
    {
        var queryParams = new
        {
            Id = customerId,
        };
        
        return await dbConnection.ExecuteAsync(DeleteCustomerById, queryParams, dbTransaction);
    }
    
    private const string DeleteCustomerById =
        """
        DELETE FROM Customers WHERE Id=@Id;
        """;
}
