using System.Text.Encodings.Web;
using System.Text.Json;

namespace BookCatalog;

public enum LoadStatus { Loaded, NotFound, Corrupt }

/// <summary>Result of loading the data file (a record: status plus the books).</summary>
public record LoadResult(LoadStatus Status, List<Book> Books);

/// <summary>
/// Reads and writes the catalog as JSON using System.Text.Json.
/// C# feature: the JSON is converted straight into typed Book objects.
/// </summary>
public static class Storage
{
    private static readonly JsonSerializerOptions Options = new()
    {
        // Property "Title" is written as "title", matching the shared JSON format.
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        // Keep characters such as ' and & readable instead of escaping them.
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public static LoadResult Load(string path)
    {
        if (!File.Exists(path))
            return new LoadResult(LoadStatus.NotFound, new List<Book>());

        try
        {
            var books = JsonSerializer.Deserialize<List<Book>>(File.ReadAllText(path), Options);

            bool allValid = books is not null && books.All(Validation.IsValidBook);
            bool idsUnique = allValid && books!.Select(b => b.Id).Distinct().Count() == books!.Count;

            return allValid && idsUnique
                ? new LoadResult(LoadStatus.Loaded, books!)
                : new LoadResult(LoadStatus.Corrupt, new List<Book>());
        }
        catch (Exception ex) when (ex is JsonException or IOException or UnauthorizedAccessException)
        {
            return new LoadResult(LoadStatus.Corrupt, new List<Book>());
        }
    }

    /// <summary>Writes the catalog to disk. Returns false if the file cannot be written.</summary>
    public static bool Save(string path, IEnumerable<Book> books)
    {
        try
        {
            string? folder = Path.GetDirectoryName(Path.GetFullPath(path));
            if (!string.IsNullOrEmpty(folder))
                Directory.CreateDirectory(folder);

            File.WriteAllText(path, JsonSerializer.Serialize(books.ToList(), Options) + "\n");
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return false;
        }
    }
}
