using FluentAssertions;
using TippingApp.Application.Common;
using TippingApp.Application.DTOs;

namespace TippingApp.Tests;

public class TipCalculationServiceTests
{
    private static IReadOnlyList<(int EmployeeId, string Name, string Role, decimal Hours)> MakeHours(
        params (string name, decimal hours)[] entries)
    {
        return entries
            .Select((e, i) => (i + 1, e.name, "Waiter", e.hours))
            .ToList()
            .AsReadOnly();
    }

    [Fact]
    public void ProportionalSplit_BasicTwoEmployees()
    {
        var hours = MakeHours(("Alice", 20m), ("Bob", 10m));

        var result = TipCalculator.CalculateTipSplit(300m, hours);

        result.Should().HaveCount(2);
        result[0].TipShare.Should().Be(200m);
        result[1].TipShare.Should().Be(100m);
    }

    [Fact]
    public void ProportionalSplit_SingleEmployee_GetsAllTips()
    {
        var hours = MakeHours(("Alice", 40m));

        var result = TipCalculator.CalculateTipSplit(500m, hours);

        result.Should().HaveCount(1);
        result[0].TipShare.Should().Be(500m);
        result[0].Percentage.Should().Be(100m);
    }

    [Fact]
    public void ProportionalSplit_ZeroHours_NoTips()
    {
        var hours = MakeHours(("Alice", 0m), ("Bob", 0m));

        var result = TipCalculator.CalculateTipSplit(300m, hours);

        result.Should().HaveCount(2);
        result.Should().AllSatisfy(e => e.TipShare.Should().Be(0));
    }

    [Fact]
    public void ProportionalSplit_ZeroTips_NobodyGetsPaid()
    {
        var hours = MakeHours(("Alice", 20m), ("Bob", 10m));

        var result = TipCalculator.CalculateTipSplit(0m, hours);

        result.Should().AllSatisfy(e => e.TipShare.Should().Be(0));
    }

    [Fact]
    public void ProportionalSplit_ThreeEmployees_CorrectPercentages()
    {
        var hours = MakeHours(("Alice", 10m), ("Bob", 10m), ("Carol", 20m));

        var result = TipCalculator.CalculateTipSplit(400m, hours);

        result[0].TipShare.Should().Be(100m);
        result[0].Percentage.Should().Be(25m);
        result[1].TipShare.Should().Be(100m);
        result[2].TipShare.Should().Be(200m);
        result[2].Percentage.Should().Be(50m);
    }

    [Fact]
    public void ProportionalSplit_UnevenSplit_RoundsTo2Decimals()
    {
        var hours = MakeHours(("Alice", 10m), ("Bob", 10m), ("Carol", 10m));

        var result = TipCalculator.CalculateTipSplit(100m, hours);

        result.Sum(r => r.TipShare).Should().BeApproximately(100m, 0.01m);
        result[0].TipShare.Should().Be(33.33m);
    }

    [Fact]
    public void ProportionalSplit_OneEmployeeZeroHours_GetsNothing()
    {
        var hours = MakeHours(("Alice", 20m), ("Bob", 0m));

        var result = TipCalculator.CalculateTipSplit(200m, hours);

        result[0].TipShare.Should().Be(200m);
        result[1].TipShare.Should().Be(0m);
    }
}
