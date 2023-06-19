namespace Pillepalle1.StatefulApi.WebApi.Converters;

public static class CqrsConverterExtensions
{
    public static CreateBookCmd ToCqrs(this CreateBookRequest request) => new()
    {
        Isbn = request.Isbn,
        Title = request.Title
    };

    public static UpdateBookCmd ToCqrs(this UpdateBookRequest request, string isbn) => new()
    {
        Isbn = isbn,
        Title = request.Title
    };
}