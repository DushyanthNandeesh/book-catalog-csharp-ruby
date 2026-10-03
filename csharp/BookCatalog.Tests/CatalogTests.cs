using Xunit;

namespace BookCatalog.Tests;

public class CatalogTests
{
    [Fact]
    public void AddAssignsNextIdAndTrims()
    {
        Catalog catalog = TestData.SampleCatalog();
        Book book = catalog.Add("  Emma ", " Jane Austen", "Romance ", 1815);
        Assert.Equal(6, book.Id);
        Assert.Equal("Emma", book.Title);
        Assert.Equal(6, catalog.Count);
    }

    [Fact]
    public void FirstIdInEmptyCatalogIsOne()
    {
        Assert.Equal(1, new Catalog().Add("A", "B", "C", 2000).Id);
    }

    [Fact]
    public void AddRejectsInvalidData()
    {
        var catalog = new Catalog();
        Assert.Throws<ArgumentException>(() => catalog.Add("", "B", "C", 2000));
        Assert.Throws<ArgumentException>(() => catalog.Add("A", "B", "C", 999));
        Assert.Equal(0, catalog.Count);
    }

    [Fact]
    public void RemoveWorksOnlyForExistingIds()
    {
        Catalog catalog = TestData.SampleCatalog();
        Assert.True(catalog.Remove(2));
        Assert.False(catalog.Remove(2));
        Assert.False(catalog.Remove(999));
        Assert.Equal("1,3,4,5", string.Join(",", catalog.ListAll().Select(b => b.Id)));
    }

    [Fact]
    public void IdAfterRemovingHighestIsHighestPlusOne()
    {
        Catalog catalog = TestData.SampleCatalog();
        catalog.Remove(5);
        Assert.Equal(5, catalog.Add("X", "Y", "Z", 2000).Id);
    }

    [Fact]
    public void SearchIsCaseInsensitivePartialAndSortedByTitle()
    {
        List<Book> results = TestData.SampleCatalog().SearchByTitle("THE");
        Assert.Equal("The Fellowship of the Ring,The Hobbit", string.Join(",", results.Select(b => b.Title)));
        Assert.Equal("Dune", TestData.SampleCatalog().SearchByTitle("  dUn ").Single().Title);
    }

    [Fact]
    public void SearchByAuthorAndGenre()
    {
        Assert.Equal(2, TestData.SampleCatalog().SearchByAuthor("orwell").Count);
        Assert.Equal(2, TestData.SampleCatalog().SearchByGenre("FANTASY").Count);
        Assert.Empty(TestData.SampleCatalog().SearchByGenre("romance"));
    }

    [Fact]
    public void GenreReportGroupsIgnoringCaseAndSorts()
    {
        List<BookGroup> groups = TestData.SampleCatalog().GroupByGenre();
        Assert.Equal("Dystopian,Fantasy,Political Satire,Science Fiction", string.Join(",", groups.Select(g => g.Name)));
        BookGroup fantasy = groups.Single(g => g.Name == "Fantasy");
        Assert.Equal("1,5", string.Join(",", fantasy.Books.Select(b => b.Id))); // sorted by year: 1937, 1954
    }

    [Fact]
    public void AuthorReportCounts()
    {
        List<BookGroup> groups = TestData.SampleCatalog().GroupByAuthor();
        Assert.Equal(2, groups.Single(g => g.Name == "George Orwell").Books.Count);
        Assert.Equal(1, groups.Single(g => g.Name == "Frank Herbert").Books.Count);
    }

    [Fact]
    public void CatalogWorksWithLinqBecauseItIsEnumerable()
    {
        Catalog catalog = TestData.SampleCatalog();
        Assert.Equal(5, catalog.Select(b => b.Id).Count());
        Assert.Equal(1949, catalog.First(b => b.Title == "1984").Year);
    }
}
