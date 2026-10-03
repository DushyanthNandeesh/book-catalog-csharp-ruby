using System.Diagnostics;
using BookCatalog;

// Performance benchmark (C#). Builds a large catalog and times the main operations.
// Run from the repository root:  dotnet run -c Release --project csharp/Benchmark
const int Count = 100_000; // books
const int Runs = 20;       // repeats for search and report timings

// Returns the average time in milliseconds for the action.
static double AverageMs(int runs, Action action)
{
    var watch = Stopwatch.StartNew();
    for (int i = 0; i < runs; i++) action();
    return watch.Elapsed.TotalMilliseconds / runs;
}

// Same data generator as the Ruby benchmark, so both languages do identical work.
var books = Enumerable.Range(1, Count).Select(i => new Book
{
    Id = i,
    Title = $"Book {i % 5000} Vol {i}",
    Author = $"Author {i % 800}",
    Genre = $"Genre {i % 40}",
    Year = 1900 + (i % 120)
}).ToList();
var catalog = new Catalog(books);

// Warm-up: lets the .NET JIT compiler prepare the code before timing starts.
catalog.SearchByTitle("vol 99");
catalog.GroupByGenre();
catalog.GroupByAuthor();

double searchMs = AverageMs(Runs, () => catalog.SearchByTitle("vol 99"));
double genreMs = AverageMs(Runs, () => catalog.GroupByGenre());
double authorMs = AverageMs(Runs, () => catalog.GroupByAuthor());

string path = Path.Combine(Path.GetTempPath(), $"big-{Guid.NewGuid()}.json");
try
{
    Storage.Save(path, catalog);
    Storage.Load(path); // warm-up
    double saveMs = AverageMs(3, () => Storage.Save(path, catalog));
    double loadMs = AverageMs(3, () => Storage.Load(path));

    Console.WriteLine($"C# .NET {Environment.Version} books={Count}");
    Console.WriteLine($"  search by title (avg of {Runs}): {searchMs,8:F1} ms");
    Console.WriteLine($"  genre report    (avg of {Runs}): {genreMs,8:F1} ms");
    Console.WriteLine($"  author report   (avg of {Runs}): {authorMs,8:F1} ms");
    Console.WriteLine($"  save JSON       (avg of 3):  {saveMs,8:F1} ms");
    Console.WriteLine($"  load JSON       (avg of 3):  {loadMs,8:F1} ms");
}
finally
{
    File.Delete(path);
}
