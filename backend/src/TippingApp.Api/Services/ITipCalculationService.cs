using TippingApp.Api.DTOs;

namespace TippingApp.Api.Services;

public interface ITipCalculationService
{
    Task<WeeklySummaryDto> GetWeeklySummaryAsync(DateOnly weekStart);
    decimal CalculateHoursForShifts(IEnumerable<Models.Shift> shifts);
    IReadOnlyList<EmployeeSummaryDto> CalculateTipSplit(
        decimal totalTips,
        IReadOnlyList<(int EmployeeId, string Name, string Role, decimal Hours)> employeeHours);
}
