using System.Collections;

namespace BookCatalog;

/// <summary>
/// The collection of books plus add, remove, search, and report operations.
/// C# features: implements IEnumerable&lt;Book&gt; so LINQ works directly on a Catalog,
/// and every query below is written with LINQ.
/// </summary>
public class Catalog : IEnumerable<Book>
{
    private readonly List<Book> _books;

    public Catalog(IEnumerable<Book>? books = null)
    {
        _books = books?.ToList() ?? new List<Book>();
    }

    public int Count => _books.Count;

    // ----- IEnumerable<Book> -----
    public IEnumerator<Book> GetEnumerator() => _books.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    // ----- Add and remove -----

    /// <summary>
    /// Adds a book with the next ID (highest existing ID plus one).
    /// Throws ArgumentException if the values break the shared validation rules.
    /// </summary>
    public Book Add(string title, string author, string genre, int year)
    {
        if (!Validation.IsValidText(title) || !Validation.IsValidText(author)
            || !Validation.IsValidText(genre) || !Validation.IsValidYear(year))
        {
            throw new ArgumentException("Invalid book data.");
        }

        int nextId = (_books.Count == 0 ? 0 : _books.Max(b => b.Id)) + 1;
        var book = new Book
        {
            Id = nextId,
            Title = title.Trim(),
            Author = author.Trim(),
            Genre = genre.Trim(),
            Year = year
        };
        _books.Add(book);
        return book;
    }

    /// <summary>Removes the book with the given ID. Returns false if there is none.</summary>
    public bool Remove(int id) => _books.RemoveAll(b => b.Id == id) > 0;

    /// <summary>All books ordered by ID.</summary>
    public List<Book> ListAll() => _books.OrderBy(b => b.Id).ToList();

    // ----- Search -----

    public List<Book> SearchByTitle(string query) => Search(b => b.Title, query);
    public List<Book> SearchByAuthor(string query) => Search(b => b.Author, query);
    public List<Book> SearchByGenre(string query) => Search(b => b.Genre, query);

    /// <summary>
    /// Case-insensitive partial match on one field, sorted by title.
    /// The field is passed in as a Func (a lambda), so one method serves all three searches.
    /// </summary>
    private List<Book> Search(Func<Book, string> field, string query)
    {
        string needle = query.Trim();
        return _books
            .Where(b => field(b).Contains(needle, StringComparison.OrdinalIgnoreCase))
            .OrderBy(b => b.Title, StringComparer.OrdinalIgnoreCase)
            .ThenBy(b => b.Id)
            .ToList();
    }

    // ----- Reports -----

    public List<BookGroup> GroupByGenre() => Group(b => b.Genre);
    public List<BookGroup> GroupByAuthor() => Group(b => b.Author);

    /// <summary>
    /// Groups books (ignoring case), sorts groups alphabetically,
    /// and sorts each group's books by year, then title.
    /// </summary>
    private List<BookGroup> Group(Func<Book, string> key) =>
        _books
            .GroupBy(key, StringComparer.OrdinalIgnoreCase)
            .OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase)
            .Select(g => new BookGroup(
                g.Key,
                g.OrderBy(b => b.Year)
                 .ThenBy(b => b.Title, StringComparer.OrdinalIgnoreCase)
                 .ThenBy(b => b.Id)
                 .ToList()))
            .ToList();
}
