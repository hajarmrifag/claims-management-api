using Claims.Domain.Entities;
using Claims.Domain.Enums;

namespace Claims.UnitTests;

public class ClaimTests
{
    [Fact]
    public void NewClaim_ShouldStartWithSubmittedStatus()
    {
        var claim = new Claim(
            "CLM-1001",
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Test claim",
            1500m);

        Assert.Equal(ClaimStatus.Submitted, claim.Status);
    }

    [Fact]
    public void UpdateStatus_ShouldChangeClaimStatus()
    {
        var claim = new Claim(
            "CLM-1002",
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Status test claim",
            2000m);

        claim.UpdateStatus(ClaimStatus.Approved);

        Assert.Equal(ClaimStatus.Approved, claim.Status);
        Assert.NotNull(claim.UpdatedAt);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(999)]
    public void UndefinedStatus_ShouldNotMutateClaim(int status)
    {
        var claim = new Claim("CLM-invalid", Guid.NewGuid(), Guid.NewGuid(), "Test", 1m);
        Assert.Throws<ArgumentOutOfRangeException>(() => claim.UpdateStatus((ClaimStatus)status));
        Assert.Equal(ClaimStatus.Submitted, claim.Status);
        Assert.Null(claim.UpdatedAt);
    }
}
