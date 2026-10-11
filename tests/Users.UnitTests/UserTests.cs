using Users.Domain.Entities;

namespace Users.UnitTests;

public sealed class UserTests
{
    [Fact]
    public void Create_NormalizesEmail_AndTrimsName()
    {
        var user = User.Create("  John.Doe@Example.COM ", "hash", "  John Doe ");

        Assert.Equal("john.doe@example.com", user.Email);
        Assert.Equal("John Doe", user.FullName);
        Assert.True(user.IsActive);
        Assert.Null(user.UpdatedAtUtc);
    }

    [Theory]
    [InlineData("", "hash", "Name")]
    [InlineData("a@b.c", " ", "Name")]
    [InlineData("a@b.c", "hash", "")]
    public void Create_RejectsMissingValues(string email, string hash, string name)
        => Assert.Throws<ArgumentException>(() => User.Create(email, hash, name));

    [Fact]
    public void Deactivate_MarksUserInactive_AndStampsUpdate()
    {
        var user = User.Create("a@b.c", "hash", "Name");

        user.Deactivate();

        Assert.False(user.IsActive);
        Assert.NotNull(user.UpdatedAtUtc);
    }

    [Fact]
    public void SoftDelete_FlagsEntity()
    {
        var user = User.Create("a@b.c", "hash", "Name");

        user.SoftDelete();

        Assert.True(user.IsDeleted);
    }
}
