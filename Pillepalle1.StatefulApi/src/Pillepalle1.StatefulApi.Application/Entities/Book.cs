namespace Pillepalle1.StatefulApi.Application.Entities;

public class Book
{
    public static readonly Book Empty = new Book()
    {
        Isbn = "000-0000000000",
        Title = String.Empty
    };
    
    public required string Isbn { init; get; }
    public required string Title { init; get; }
}