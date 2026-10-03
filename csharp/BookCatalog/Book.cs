namespace BookCatalog;

/// <summary>
/// A single book in the catalog.
/// Properties use "init" setters, so a Book cannot be changed after it is created.
/// C# feature: strongly typed properties (int, string) checked at compile time.
/// </summary>
public class Book
{
    public int Id { get; init; }
    public string Title { get; init; } = "";
    public string Author { get; init; } = "";
    public string Genre { get; init; } = "";
    public int Year { get; init; }

    /// <summary>Shared display format: [id] Title | Author | Genre | Year</summary>
    public override string ToString() => $"[{Id}] {Title} | {Author} | {Genre} | {Year}";
}

/// <summary>
/// One group in a report (for example, all books of one genre).
/// C# feature: a record is a compact, immutable data type with built-in equality.
/// </summary>
public record BookGroup(string Name, List<Book> Books);
