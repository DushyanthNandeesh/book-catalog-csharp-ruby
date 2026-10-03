using Xunit;

namespace BookCatalog.Tests;

public class StorageTests : IDisposable
{
    private readonly string _folder = Path.Combine(Path.GetTempPath(), "bookcatalog-" + Guid.NewGuid());

    public StorageTests() => Directory.CreateDirectory(_folder);

    // Runs after each test: remove the temporary folder.
    public void Dispose() => Directory.Delete(_folder, recursive: true);

    private string TempPath() => Path.Combine(_folder, "books.json");

    [Fact]
    public void MissingFile()
    {
        LoadResult result = Storage.Load(TempPath());
        Assert.Equal(LoadStatus.NotFound, result.Status);
        Assert.Empty(result.Books);
    }

    [Fact]
    public void SaveThenLoadRoundTrip()
    {
        string path = TempPath();
        Assert.True(Storage.Save(path, TestData.SampleCatalog()));

        LoadResult result = Storage.Load(path);
        Assert.Equal(LoadStatus.Loaded, result.Status);
        Assert.Equal(
            string.Join("|", TestData.SampleCatalog().Select(b => b.ToString())),
            string.Join("|", result.Books.Select(b => b.ToString())));
    }

    [Fact]
    public void SaveCreatesMissingFolder()
    {
        string path = Path.Combine(_folder, "new_folder", "books.json");
        Assert.True(Storage.Save(path, new List<Book>()));
        Assert.Equal("[]\n", File.ReadAllText(path));
    }

    [Fact]
    public void CorruptFilesAreDetected()
    {
        string[] badFiles =
        {
            "not json", "", "{\"id\": 1}",
            "[{\"id\":1,\"title\":\"A\",\"author\":\"B\",\"genre\":\"C\"}]",
            "[{\"id\":1,\"title\":\"A\",\"author\":\"B\",\"genre\":\"C\",\"year\":\"1990\"}]",
            "[{\"id\":1,\"title\":\"A\",\"author\":\"B\",\"genre\":\"C\",\"year\":3000}]",
            "[{\"id\":1,\"title\":\"A\",\"author\":\"B\",\"genre\":\"C\",\"year\":1990},{\"id\":1,\"title\":\"D\",\"author\":\"E\",\"genre\":\"F\",\"year\":1991}]",
        };

        foreach (string contents in badFiles)
        {
            string path = TempPath();
            File.WriteAllText(path, contents);
            Assert.True(LoadStatus.Corrupt == Storage.Load(path).Status, $"expected corrupt: {contents}");
        }
    }
}
