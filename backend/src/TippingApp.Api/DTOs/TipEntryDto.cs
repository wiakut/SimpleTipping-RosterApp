namespace TippingApp.Api.DTOs;

public record TipEntryReadDto(
    int Id,
    DateOnly Date,
    decimal Amount,
    string Source
);

public record TipEntryCreateDto(
    DateOnly Date,
    decimal Amount,
    string Source
);

public record TipEntryUpdateDto(
    DateOnly Date,
    decimal Amount,
    string Source
);
