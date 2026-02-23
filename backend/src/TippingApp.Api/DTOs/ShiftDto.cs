namespace TippingApp.Api.DTOs;

public record ShiftReadDto(
    int Id,
    int EmployeeId,
    string EmployeeName,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime
);

public record ShiftCreateDto(
    int EmployeeId,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime
);

public record ShiftUpdateDto(
    int EmployeeId,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime
);
