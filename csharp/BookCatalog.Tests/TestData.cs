namespace BookCatalog.Tests;

/// <summary>Builds the small catalog used by several tests.</summary>
internal static class TestData
{
    public static Catalog SampleCatalog() => new(new List<Book>
    {
        new() { Id = 1, Title = "The Hobbit", Author = "J.R.R. Tolkien", Genre = "Fantasy", Year = 1937 },
        new() { Id = 2, Title = "1984", Author = "George Orwell", Genre = "Dystopian", Year = 1949 },
        new() { Id = 3, Title = "Animal Farm", Author = "George Orwell", Genre = "Political Satire", Year = 1945 },
        new() { Id = 4, Title = "Dune", Author = "Frank Herbert", Genre = "Science Fiction", Year = 1965 },
        new() { Id = 5, Title = "The Fellowship of the Ring", Author = "J.R.R. Tolkien", Genre = "fantasy", Year = 1954 },
    });
}
