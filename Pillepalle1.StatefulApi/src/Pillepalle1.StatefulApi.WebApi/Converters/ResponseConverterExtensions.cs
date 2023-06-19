using Pillepalle1.StatefulApi.Application.Entities;

namespace Pillepalle1.StatefulApi.WebApi.Converters;

public static class ResponseConverterExtensions
{
    public static BookResponse ToResponse(this Book book) => new()
    {
        Isbn = book.Isbn,
        Title = book.Title
    };
}