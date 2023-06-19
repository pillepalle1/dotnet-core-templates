namespace Pillepalle1.StatefulApi.WebApi.Contracts;

public class CreateBookRequest
{
    public required string Isbn { get; set; }
    public required string Title { get; set; }
}