namespace TippingApp.Api.DTOs;

public record WeeklySummaryDto(
    DateOnly WeekStart,
    DateOnly WeekEnd,
    decimal TotalTips,
    decimal TotalHours,
    IReadOnlyList<EmployeeSummaryDto> Employees
);

public record EmployeeSummaryDto(
    int EmployeeId,
    string Name,
    string Role,
    decimal HoursWorked,
    decimal TipShare,
    decimal Percentage
);

public record EmployeeReadDto(
    int Id,
    string Name,
    string Role
);
