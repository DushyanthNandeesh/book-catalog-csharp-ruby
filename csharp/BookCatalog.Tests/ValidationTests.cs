using Xunit;

namespace BookCatalog.Tests;

public class ValidationTests
{
    [Fact]
    public void ValidText()
    {
        Assert.True(Validation.IsValidText("Dune"));
        Assert.False(Validation.IsValidText(""));
        Assert.False(Validation.IsValidText("   "));
        Assert.False(Validation.IsValidText(null)); // the compiler tracks null, but we still check at run time
    }

    [Fact]
    public void ValidYearRange()
    {
        Assert.True(Validation.IsValidYear(1000));
        Assert.True(Validation.IsValidYear(DateTime.Now.Year));
        Assert.False(Validation.IsValidYear(999));
        Assert.False(Validation.IsValidYear(DateTime.Now.Year + 1));
    }

    [Fact]
    public void ParseIntAcceptsDigitsOnly()
    {
        Assert.True(Validation.TryParseInt(" 1990 ", out int value));
        Assert.Equal(1990, value);
        Assert.True(Validation.TryParseInt("007", out value));
        Assert.Equal(7, value);

        foreach (string? bad in new string?[] { "", "abc", "+5", "-5", "1_000", "19.5", "99999999999999999999", null })
            Assert.False(Validation.TryParseInt(bad, out _), $"expected '{bad}' to be rejected");
    }
}
