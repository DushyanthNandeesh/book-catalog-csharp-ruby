namespace BookCatalog;

/// <summary>Console menu for the Book Cataloging System (C# version).</summary>
public static class Program
{
    public static void Main(string[] args)
    {
        // Optional first argument: path to the JSON data file.
        string path = args.Length > 0 ? args[0] : FindDefaultDataPath();

        Catalog catalog = LoadCatalog(path);
        RunMenu(catalog);

        Console.WriteLine(Storage.Save(path, catalog)
            ? "Catalog saved. Goodbye."
            : "Error: could not save the catalog.");
    }

    // ----- Startup -----

    /// <summary>Looks for data/books.json in the current folder and each parent folder.</summary>
    private static string FindDefaultDataPath()
    {
        var folder = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (folder != null)
        {
            string candidate = Path.Combine(folder.FullName, "data", "books.json");
            if (File.Exists(candidate)) return candidate;
            folder = folder.Parent;
        }
        return Path.Combine("..", "data", "books.json");
    }

    private static Catalog LoadCatalog(string path)
    {
        LoadResult result = Storage.Load(path);
        switch (result.Status)
        {
            case LoadStatus.NotFound:
                Console.WriteLine("Data file not found. Starting with an empty catalog.");
                break;
            case LoadStatus.Corrupt:
                Console.WriteLine("Warning: could not read data file. Starting with an empty catalog.");
                break;
        }
        return new Catalog(result.Books);
    }

    // ----- Menu loop -----

    private static void RunMenu(Catalog catalog)
    {
        while (true)
        {
            PrintMenu();
            string? choice = Console.ReadLine();
            if (choice is null) return;          // end of input behaves like "Save and exit"

            switch (choice.Trim())
            {
                case "1": AddBook(catalog); break;
                case "2": RemoveBook(catalog); break;
                case "3": ShowBooks("All Books", catalog.ListAll()); break;
                case "4": Search("title", catalog.SearchByTitle); break;
                case "5": Search("author", catalog.SearchByAuthor); break;
                case "6": Search("genre", catalog.SearchByGenre); break;
                case "7": ShowReport("Books by Genre", catalog.GroupByGenre()); break;
                case "8": ShowReport("Books by Author", catalog.GroupByAuthor()); break;
                case "9": return;
                default: Console.WriteLine("Invalid input."); break;
            }
        }
    }

    private static void PrintMenu()
    {
        Console.WriteLine();
        Console.WriteLine("=== Book Catalog ===");
        Console.WriteLine("1. Add a book");
        Console.WriteLine("2. Remove a book");
        Console.WriteLine("3. List all books");
        Console.WriteLine("4. Search by title");
        Console.WriteLine("5. Search by author");
        Console.WriteLine("6. Search by genre");
        Console.WriteLine("7. Report: books by genre");
        Console.WriteLine("8. Report: books by author");
        Console.WriteLine("9. Save and exit");
        Console.Write("Choose an option: ");
    }

    private static string Prompt(string label)
    {
        Console.Write(label);
        return Console.ReadLine() ?? "";
    }

    // ----- Menu actions -----

    private static void AddBook(Catalog catalog)
    {
        string title = Prompt("Title: ");
        string author = Prompt("Author: ");
        string genre = Prompt("Genre: ");
        string yearText = Prompt("Year: ");

        if (!Validation.TryParseInt(yearText, out int year)
            || !Validation.IsValidText(title) || !Validation.IsValidText(author)
            || !Validation.IsValidText(genre) || !Validation.IsValidYear(year))
        {
            Console.WriteLine("Invalid input.");
            return;
        }

        catalog.Add(title, author, genre, year);
        Console.WriteLine("Book added.");
    }

    private static void RemoveBook(Catalog catalog)
    {
        if (!Validation.TryParseInt(Prompt("Book ID to remove: "), out int id))
        {
            Console.WriteLine("Invalid input.");
            return;
        }

        Console.WriteLine(catalog.Remove(id) ? "Book removed." : $"No book found with ID {id}.");
    }

    /// <summary>
    /// Runs one of the three searches. The search method is passed in as a delegate,
    /// so this one method handles title, author, and genre.
    /// </summary>
    private static void Search(string fieldName, Func<string, List<Book>> searchMethod)
    {
        string query = Prompt($"Search {fieldName}: ");
        if (!Validation.IsValidText(query))
        {
            Console.WriteLine("Invalid input.");
            return;
        }
        ShowBooks("Search Results", searchMethod(query));
    }

    // ----- Output -----

    private static void ShowBooks(string heading, List<Book> books)
    {
        if (books.Count == 0)
        {
            Console.WriteLine("No books found.");
            return;
        }
        Console.WriteLine($"=== {heading} ===");
        books.ForEach(b => Console.WriteLine(b));
    }

    private static void ShowReport(string heading, List<BookGroup> groups)
    {
        if (groups.Count == 0)
        {
            Console.WriteLine("No books found.");
            return;
        }
        Console.WriteLine($"=== {heading} ===");
        foreach (BookGroup group in groups)
        {
            Console.WriteLine($"{group.Name} ({group.Books.Count})");
            foreach (Book book in group.Books)
                Console.WriteLine($"  {book}");
        }
    }
}
