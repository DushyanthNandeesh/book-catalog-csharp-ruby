using System.Globalization;

namespace BookCatalog;

/// <summary>
/// Input rules from the shared specification. Used by both the menu and the file loader.
/// </summary>
public static class Validation
{
    public const int MinYear = 1000;

    /// <summary>Text fields must contain something other than whitespace.</summary>
    public static bool IsValidText(string? value) => !string.IsNullOrWhiteSpace(value);

    /// <summary>Year must be a whole number from 1000 through the current year.</summary>
    public static bool IsValidYear(int year) => year >= MinYear && year <= DateTime.Now.Year;

    /// <summary>
    /// Parses digits only (no sign, no decimals). NumberStyles.None rejects everything else.
    /// Values too large for a 32-bit int also fail, so the whole program uses one rule.
    /// </summary>
    public static bool TryParseInt(string? text, out int value) =>
        int.TryParse(text?.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out value);

    /// <summary>Checks a book that was read from the JSON file.</summary>
    public static bool IsValidBook(Book? book) =>
        book is not null
        && book.Id > 0
        && IsValidText(book.Title)
        && IsValidText(book.Author)
        && IsValidText(book.Genre)
        && IsValidYear(book.Year);
}
