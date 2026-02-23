using FluentAssertions;
using TippingApp.Application.Common;
using TippingApp.Domain.Entities;

namespace TippingApp.Tests;

public class WeeklyHoursCalculationTests
{
    private static Shift MakeShift(int hour1, int min1, int hour2, int min2) => new()
    {
        Id = 1,
        EmployeeId = 1,
        Date = new DateOnly(2025, 1, 6),
        StartTime = new TimeOnly(hour1, min1),
        EndTime = new TimeOnly(hour2, min2)
    };

    [Fact]
    public void StandardShift_8Hours()
    {
        var shifts = new[] { MakeShift(9, 0, 17, 0) };

        var hours = TipCalculator.CalculateHoursForShifts(shifts);

        hours.Should().Be(8m);
    }

    [Fact]
    public void ShortShift_4AndHalfHours()
    {
        var shifts = new[] { MakeShift(10, 0, 14, 30) };

        var hours = TipCalculator.CalculateHoursForShifts(shifts);

        hours.Should().Be(4.5m);
    }

    [Fact]
    public void MultipleShifts_SumsCorrectly()
    {
        var shifts = new[]
        {
            MakeShift(9, 0, 13, 0),
            MakeShift(14, 0, 18, 0),
            MakeShift(9, 0, 17, 0)
        };

        var hours = TipCalculator.CalculateHoursForShifts(shifts);

        hours.Should().Be(16m);
    }

    [Fact]
    public void NoShifts_ZeroHours()
    {
        var hours = TipCalculator.CalculateHoursForShifts([]);

        hours.Should().Be(0m);
    }

    [Fact]
    public void EveningShift_CorrectHours()
    {
        var shifts = new[] { MakeShift(16, 0, 23, 30) };

        var hours = TipCalculator.CalculateHoursForShifts(shifts);

        hours.Should().Be(7.5m);
    }

    [Fact]
    public void ShiftWithOddMinutes_CalculatesCorrectly()
    {
        var shifts = new[] { MakeShift(9, 15, 15, 45) };

        var hours = TipCalculator.CalculateHoursForShifts(shifts);

        hours.Should().Be(6.5m);
    }
}
